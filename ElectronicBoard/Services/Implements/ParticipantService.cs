using Azure;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ElectronicBoard.Services.Implements
{
    public class ParticipantService : IParticipantService
    {
		// ДОБАВИТЬ Метод авторизации
		// ПОЛУЧЕНИЕ ПОЛЬЗОВАТЕЛЯ ДЛЯ ВХОДА
		private IAuthorService authorService { get; set; }
		private IFileService fileService { get; set; }
		private IBlockService blockService { get; set; }
		private IBoardService boardService { get; set; }
		private IStickerService stickerService { get; set; }

        public ParticipantService(IAuthorService _authorService, 
            IFileService _fileService, IStickerService _stickerService, 
            IBlockService _blockService, IBoardService _boardService)
        {
            authorService = _authorService;
            fileService = _fileService;
            stickerService = _stickerService;
            blockService = _blockService;
            boardService = _boardService;
            CreateTestParticipant();
        }

        // Получение всего списка участников
        public async Task<List<Participant>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Participants.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

        // Получение участников по имени или id блока
        public async Task<List<Participant>> GetFilteredList(Participant? model, int? blockId)
        {
            List<Participant> participants = new List<Participant>();
            using var context = new ElectronicBoardDatabase();
            if (model == null && blockId == null)
            {
                return null;
            }
            else if (blockId != null)
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
            else
            {
                return (await context.Participants.ToListAsync())
                .Where(rec => rec.ParticipantFIO.Contains(model.ParticipantFIO))
                .Select(CreateModel)
                .ToList();
            }
        }

        // Получение участника по id или ФИО
        public async Task<Participant> GetElement(Participant model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Participants
            .FirstOrDefaultAsync(rec => rec.ParticipantFIO.Contains(model.ParticipantFIO) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

        // ПОЛУЧЕНИЕ ПОЛЬЗОВАТЕЛЯ ДЛЯ ВХОДА
		public async Task<Participant> Enter(string login, string password)
		{
			using var context = new ElectronicBoardDatabase();
			var component = await context.Participants
			.FirstOrDefaultAsync(rec => rec.ParticipantFIO.Contains(login));
			return component != null ? CreateModel(component) : null;
		}

		// Добавление участника
		public async Task Insert(Participant model)
        {
            using var context = new ElectronicBoardDatabase();
            await context.Participants.AddAsync(CreateModel(model, new Participant()));
            await context.SaveChangesAsync();

			// Добавление участника к общей доске
            if (Program.MainBoard == null)
			Program.MainBoard = await boardService.GetElement(new Board
			{
				BoardName = "Общая доска",
				BoardThematics = "Общая"
			});

			int main_block_id = (await blockService.GetElement(new Block { BoardId = Program.MainBoard.Id })).Id;
            await blockService.AddOrRemoveElement(await GetElement(model), main_block_id);
        }

        // Редактирование данных об участнике
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

        // Удаление участника
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

        private async Task CreateTestParticipant() 
        {
			/// Участник досок для тестирования приложения ///
			List<Participant> participants = await GetFullList();

			if (participants.Count == 0)
			{
				using var context = new ElectronicBoardDatabase();
				await context.Participants.AddAsync(CreateModel(new Participant
				{
					ParticipantFIO = "Проверяющий",
					ParticipantPatents = "нет",
					ParticipantPublications = "нет",
					ParticipantRating = XHirsh("Проверяющий"),
					ParticipantTasks = "нет",
					ScientificInterests = "нет",
					Picture = new byte[] { }
				}, new Participant()));
				await context.SaveChangesAsync();
			}

			Participant part = await GetElement(new Participant
			{
				ParticipantFIO = "Проверяющий"
			});
            await boardService.CreateMainBoard(part);
		}
        private static Participant CreateModel(Participant model, Participant participant)
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
                ParticipantsBlocks = participant.ParticipantsBlocks
            };
        }
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
	}
}
