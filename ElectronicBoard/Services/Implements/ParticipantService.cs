﻿using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Участник"
	/// </summary>
	public class ParticipantService : IParticipantService
    {
		private UserManager<IdentityUser<int>> _userManager;
		private SignInManager<IdentityUser<int>> _signInManager;
		private IAuthorService authorService { get; set; }
		private IFileService fileService { get; set; }
		private IBlockService blockService { get; set; }
		private IBoardService boardService { get; set; }
		private IStickerService stickerService { get; set; }

		public ParticipantService(UserManager<IdentityUser<int>> user, SignInManager<IdentityUser<int>> signIn,
			IAuthorService _authorService, 
            IFileService _fileService, IStickerService _stickerService, 
            IBlockService _blockService, IBoardService _boardService)
        {
			_userManager = user;
			_signInManager = signIn;
            authorService = _authorService;
            fileService = _fileService;
            stickerService = _stickerService;
            blockService = _blockService;
            boardService = _boardService;
		}

		public ParticipantService() {}

		/// <summary>
		/// Метод для получения списка участников
		/// </summary>
		/// <returns></returns>
		public async Task<List<Participant>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Participants.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения участников по Id блока
		/// </summary>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public async Task<List<Participant>> GetFilteredList(int? blockId)
		{
			List<Participant> participants = new List<Participant>();
			using var context = new ElectronicBoardDatabase();
			if (blockId == null)
			{
				return new List<Participant>();
			}
			else
			{
				var block_parts = (await context.BlockParticipants.ToListAsync()).Where(rec => rec.BlockId == blockId);
				if (block_parts != null)
				{
					foreach (var block_part in block_parts)
					{
						var participant = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == block_part.ParticipantId);
						if (participant != null) participants.Add(participant);
					}
				}
				return participants;
			}
		}

		/// <summary>
		/// Метод для проверки логина/пароля участника
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public async Task<Participant?> Enter(string login, string password)
		{
			using var context = new ElectronicBoardDatabase();
			var component = await context.Participants
			.FirstOrDefaultAsync(rec => rec.ParticipantFIO.Contains(login));
			return component != null ? CreateModel(component) : null;
		}

		/// <summary>
		/// Метод для получения участника по Id или ФИО
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Participant?> GetElement(Participant model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
			Participant? component = null;
			if (model.Id > 0)
			{
				component = await context.Participants
					.FirstOrDefaultAsync(rec => rec.Id == model.Id);
			}
			else if (model.IdentityId > 0) 
			{
				List<Participant> parts = await GetFullList();
				component = await context.Participants
					.FirstOrDefaultAsync(rec => rec.IdentityId == model.IdentityId);
			}
			else
			{
				component = await context.Participants
					.FirstOrDefaultAsync(rec => rec.ParticipantFIO.Contains(model.ParticipantFIO));
			}
            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления участника
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Participant model)
        {
            using var context = new ElectronicBoardDatabase();
            await context.Participants.AddAsync(CreateModel(model, new Participant()));
            await context.SaveChangesAsync();

			var new_part = await GetElement(new Participant { ParticipantFIO = model.ParticipantFIO });

			// Поиск общей доски
            var main_board = await boardService.GetElement(new Board { BoardName = "Общая доска" });

			// Если общая доска создана, но ссылка не указана - указать на неё ссылку в Program
			if (main_board != null) 
			{
				if (Program.MainBoard == null) Program.MainBoard = main_board;

				// Блок участников общей доски, добавление в него нового участника
				Block? main_block = await blockService.GetElement(new Block { BlockName = "Участники", BoardId = Program.MainBoard.Id });
				if (main_block != null) await blockService.AddOrRemoveElement(new_part, main_block.Id);
			}

			// Если общая доска ещё не создана - создание общей доски и добавление в неё первого участника
			else await boardService.CreateMainBoard(new_part);
        }

		/// <summary>
		/// Метод для редактирования участника
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Participant model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Участник не найден");
            }
            CreateModel(model, element);
            await context.SaveChangesAsync();
        }

		/// <summary>
		/// Метод для удаления участника
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Participant model)
        {
            using var context = new ElectronicBoardDatabase();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == model.Id);
                var part_projects = await context.ProjectParticipants.FirstOrDefaultAsync(rec => rec.ParticipantId == model.Id);
                var author = await context.Authors.FirstOrDefaultAsync(rec => rec.ParticipantId == model.Id);

                if (element != null && part_projects == null)
                {
                    // Удаление файлов
                    var files = (await context.Files.ToListAsync())
                        .Where(rec => rec.ParticipantId == model.Id)
                        .Select(fileService.CreateModel)
                        .ToList();
                    foreach (var file in files)
                    {
                        context.Files.Remove(file);
                        await context.SaveChangesAsync();
                    }

                    // Удаление стикеров
                    var stickers = (await context.Stickers.ToListAsync())
                        .Where(rec => rec.ParticipantId == model.Id)
                        .Select(stickerService.CreateModel)
                        .ToList();
                    foreach (var sticker in stickers)
                    {
                        context.Stickers.Remove(sticker);
                        await context.SaveChangesAsync();
                    }

                    // Если участник является автором статей -
                    // удаление ссылки на участника
                    if (author != null)
                    {
                        author.ParticipantId = null;
                        await authorService.Update(author);
                    }

                    // Удаление участника
                    context.Participants.Remove(element);
                    await context.SaveChangesAsync();
                }
                else if (part_projects != null)
                {
                    throw new Exception("Участник не может быть удалён");
                }
                else
                {
                    throw new Exception("Участник не найден");
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

		/// <summary>
		/// Метод для регистрации участника в приложении
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public async Task Register(Participant part)
		{
			var user = new IdentityUser<int> { UserName = part.Login };
			var result = await _userManager.CreateAsync(user, part.Password);

			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, isPersistent: false);

				List<Claim> claims = new List<Claim>();

				claims.Add(new Claim("UserID", user.Id.ToString()));
				claims.Add(new Claim("ParticipantFIO", part.ParticipantFIO));
				claims.Add(new Claim("ScientificInterests", part.ParticipantFIO));
				claims.Add(new Claim("ParticipantTasks", part.ParticipantTasks));
				claims.Add(new Claim("ParticipantRating", part.ParticipantRating.ToString()));
				claims.Add(new Claim("ParticipantPublications", part.ParticipantPublications));
				claims.Add(new Claim("ParticipantPatents", part.ParticipantPatents));

				claims.Add(new Claim("IdentityId", part.IdentityId.ToString()));
				claims.Add(new Claim("Login", part.Login));
				claims.Add(new Claim("Password", part.Password));

				await _userManager.AddClaimsAsync(user, claims);

				var new_user = await _userManager.FindByNameAsync(part.Login);
				if (new_user != null)
				{
					part.IdentityId = new_user.Id;
					await Insert(part);
					//await CreateTestParticipant(part);
				}
			}
			else
			{
				throw new Exception("Ошибка регистрации участника");
			}
		}

		/// <summary>
		/// Метод для создания тестового участника
		/// </summary>
		/// <param name="participant"></param>
		/// <returns></returns>
		public async Task CreateTestParticipant(Participant participant) 
        {
			List<Participant> participants = await GetFullList();
			
			if (participants.Count == 0)
			{
				using var context = new ElectronicBoardDatabase();
				await context.Participants.AddAsync(CreateModel(participant, new Participant()));
				await context.SaveChangesAsync();
			}

			Participant? part = await GetElement(new Participant
			{
				ParticipantFIO = participant.ParticipantFIO
			});
            if (part != null) await boardService.CreateMainBoard(part);
		}

		/// <summary>
		/// Метод для получения индекса Хирша
		/// </summary>
		/// <param name="ParticipantFIO"></param>
		/// <returns></returns>
		private static int XHirsh(string ParticipantFIO)
		{
			string path = "https://www.elibrary.ru/authors.asp?surname=" + ParticipantFIO + "&codetype=SPIN&codevalue=&town=Ульяновск&countryid=RUS&orgname=Ульяновский%20государственный%20технический%20университет&rubriccode=&metrics=1&sortorder=3&order=1";
            int indexH = 0;
			try
			{
                using (HttpClientHandler hdl = new HttpClientHandler { AllowAutoRedirect = true, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None }) 
                {
                    using (var clnt = new HttpClient(hdl)) 
                    {
                        using (HttpResponseMessage resp = clnt.GetAsync(path).Result) 
                        {
							if (resp.IsSuccessStatusCode)
							{
								var html = resp.Content.ReadAsStringAsync().Result;
								if (!string.IsNullOrEmpty(html))
								{
									HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
									doc.LoadHtml(html);

									var hirsh = doc.DocumentNode.SelectSingleNode(".//td[@class='midtext select-tr-right']//div");
									if (hirsh != null)
									{
                                        indexH = Convert.ToInt32(hirsh.InnerHtml);
									}
								}
							}
							else if (resp.StatusCode is  HttpStatusCode.Redirect)
							{
                                var loc = resp.Headers.Location;
							    var response = clnt.GetAsync(loc).Result;
								
                                if (response.IsSuccessStatusCode)
							    {
									var html = resp.Content.ReadAsStringAsync().Result;
									if (!string.IsNullOrEmpty(html))
									{
										HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
										doc.LoadHtml(html);

										var hirsh = doc.DocumentNode.SelectSingleNode(".//td[@class='midtext select-tr-right']//div");
										if (hirsh != null)
										{
											indexH = Convert.ToInt32(hirsh.InnerHtml);
										}
									}
								}
							}
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
				return indexH;
			}
			return indexH;
		}

		/// <summary>
		/// Метод для обновления рейтинга
		/// </summary>
		/// <param name="partId"></param>
		/// <returns></returns>
		public async Task UpdRaiting(int partId)
		{
			using var context = new ElectronicBoardDatabase();
			var element = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == partId);
			if (element == null)
			{
				throw new Exception("Участник не найден");
			}
            await Update(element);
		}

		public Participant CreateModel(Participant model, Participant participant)
		{
			participant.ParticipantFIO = model.ParticipantFIO;
			participant.ParticipantTasks = model.ParticipantTasks;
			participant.ScientificInterests = model.ScientificInterests;

			participant.ParticipantPublications = model.ParticipantPublications;
			participant.ParticipantRating = XHirsh(model.ParticipantFIO);
			participant.ParticipantPatents = model.ParticipantPatents;
			participant.Files = model.Files;
			participant.Picture = model.Picture;
			participant.Stikers = model.Stikers;
			participant.ParticipantsBlocks = model.ParticipantsBlocks;

			participant.IdentityId = model.IdentityId;
			participant.Login = model.Login;
			participant.Password = model.Password;

			return participant;
		}
		private static Participant CreateModel(Participant participant)
		{
			return new Participant
			{
				Id = participant.Id,

				ParticipantFIO = participant.ParticipantFIO,
				ParticipantTasks = participant.ParticipantTasks,
				ScientificInterests = participant.ScientificInterests,

				ParticipantPublications = participant.ParticipantPublications,
				ParticipantRating = participant.ParticipantRating,
				ParticipantPatents = participant.ParticipantPatents,
				Files = participant.Files,
				Picture = participant.Picture,
				Stikers = participant.Stikers,
				ParticipantsBlocks = participant.ParticipantsBlocks,

				IdentityId = participant.IdentityId,
				Login = participant.Login,
				Password = participant.Password
			};
		}
	}
}
