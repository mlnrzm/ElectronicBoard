using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicBoard.Controllers
{
	[Authorize]
	public class GrantController : Controller
	{
		private readonly ILogger<GrantController> _logger;
		private readonly INotyfService _notyf;
		private readonly IdnMapping idn;

		private readonly UserManager<IdentityUser<int>> _userManager;

		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IFileService fileService;
		private readonly IGrantService grantService;
		private readonly IParticipantService participantService;
		private readonly IStickerService stickerService;

		public GrantController(ILogger<GrantController> logger, INotyfService notyf, 
			IBoardService _boardService, IBlockService _blockService, IStickerService _stickerService, 
			IGrantService _grantService, IParticipantService _participantService, IFileService _fileService, UserManager<IdentityUser<int>> userManager)
		{
			_logger = logger;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			grantService = _grantService;
			participantService = _participantService;
			stickerService = _stickerService;
			fileService = _fileService;
			_userManager = userManager;
			idn = new IdnMapping();
		}

		// Отображение страницы с информацией о гранте
		public async Task<IActionResult> Index(Grant grant)
		{
			IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Grant find_grant = await grantService.GetElement(new Grant { Id = grant.Id, BlockId = grant.BlockId });

			if (find_grant != null)
			{
				// Конвертация изображения
				if (find_grant.Picture.Length > 0)
				{
					ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_grant.Picture);
				}

				// Стикеры
				List<Sticker> stickers = await stickerService.GetFilteredList("grant", find_grant.Id);
				ViewBag.Stickers = stickers;

				// Файлы
				List<File> files = await fileService.GetFilteredList("grant", find_grant.Id);
				ViewBag.Files = files;

				// Передача на страницу участников гранта
				ViewBag.Participants = grantService.GetParticipants(find_grant.Id);

				// Блок, на котором находится элемент
				Block find_block = await blockService.GetElement(new Block { Id = find_grant.BlockId });
				ViewBag.Block = find_block;

				// Доска, на которой находится блок
				Board board = await boardService.GetElement(new Board { Id = find_block.BoardId });
				List<Block> added_blocks = new List<Block>();
				foreach (var b in await blockService.GetFilteredList(new Block { BoardId = board.Id })) { added_blocks.Add(b); }
				ViewBag.Board = new Board
				{
					Id = board.Id,
					BoardName = board.BoardName,
					BoardThematics = board.BoardThematics,
					Blocks = added_blocks
				};

				return View(find_grant);
			}
			else
			{
				_notyf.Error("Грант не найден");
				return Redirect("javascript: history.go(-1)");
			}
		}

		// Добавление гранта
		[HttpGet]
		public async Task<IActionResult> AddGrant(string blockId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			// Передача id блока, на котором будет находиться грант
			Block block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
			if (block != null)
			{
				ViewData["blockId"] = blockId;
				return View();
			}
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task AddGrant(string blockId, string name, string desc, string source, 
			string status, string text, string datesr, string datestart, string datefinish, IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(desc) && !string.IsNullOrEmpty(source)
				&& !string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(datesr)
				&& !string.IsNullOrEmpty(datestart) && !string.IsNullOrEmpty(datefinish))
			{
				Block block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				if (block != null)
				{
					try
					{
						// Изображение
						byte[] picture = new byte[] { };
						if (pict != null)
						{
							using (var target = new MemoryStream())
							{
								await pict.CopyToAsync(target);
								picture = target.ToArray();
							}
						}

						DateTime DateStart = DateTime.ParseExact(datestart, "yyyy-M-dd", null);
						DateTime DateFinish = DateTime.ParseExact(datefinish, "yyyy-M-dd", null);
						DateTime DateSr = DateTime.ParseExact(datesr, "yyyy-M-dd", null);

						if (DateStart <= DateFinish && DateSr <= DateStart)
						{
							// Добавление и отображение гранта
							await grantService.Insert(new Grant
							{
								GrantName = name,
								GrantText = text,
								GrantSource = source,
								GrantDescription = desc,
								GrantStatus = status,
								GrantDeadline = DateSr,
								GrantDeadlineStart = DateStart,
								GrantDeadlineFinish = DateFinish,
								BlockId = block.Id,
								Picture = picture
							});
							Grant new_grant = await grantService.GetElement(new Grant
							{
								GrantName = name,
								BlockId = block.Id
							});

							Response.Redirect($"/grant/index?" +
								$"Id={idn.GetAscii(new_grant.Id.ToString())}" +
								$"&BlockId={idn.GetAscii(new_grant.BlockId.ToString())}");
						}
						else if (DateStart > DateFinish)
						{
							_notyf.Error("Дата начала должна быть раньше даты окончания.");
							Response.Redirect($"/grant/addgrant?blockId={idn.GetAscii(block.Id.ToString())}");
						}
						else if (DateSr >= DateStart)
						{
							_notyf.Error("Срок подачи заявки должен быть раньше срока начала выполнения.");
							Response.Redirect($"/grant/addgrant?blockId={idn.GetAscii(block.Id.ToString())}");
						}
						else
						{
							_notyf.Error("Ошибка в указании дат.");
							Response.Redirect($"/grant/addgrant?blockId={idn.GetAscii(block.Id.ToString())}");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/grant/addgrant?blockId={idn.GetAscii(blockId)}");
					}
				}
				else
				{
					_notyf.Error("Ошибка");
					Response.Redirect("javascript: history.go(-1)");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/grant/addgrant?blockId={idn.GetAscii(blockId)}");
			}
		}

		// Редактирование гранта
		[HttpGet]
		public async Task<IActionResult> UpdGrant(Grant grant)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Grant find_grant = await grantService.GetElement(new Grant
			{
				Id = grant.Id,
				BlockId = grant.BlockId,
				GrantName = idn.GetUnicode(grant.GrantName)
			});

			if (find_grant != null)
			{
				return View(find_grant);
			}
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task UpdGrant(string id, string blockId, 
			string name, string desc, string source, string status, 
			string text, string datesr, string datestart, string datefinish, 
			IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(desc) && !string.IsNullOrEmpty(source)
				&& !string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(datesr)
				&& !string.IsNullOrEmpty(datestart) && !string.IsNullOrEmpty(datefinish))
			{
				Block block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				Grant grant = await grantService.GetElement(new Grant { Id = Convert.ToInt32(id) });
				if (block != null && grant != null)
				{ 
					try
					{
						// ID элемента
						int ElementId = Convert.ToInt32(id);
						// ID блока
						int BlockId = Convert.ToInt32(blockId);

						// Изображение
						bool del = true;
						switch (delpic)
						{
							case "on":
								del = true;
								break;
							case null:
								del = false;
								break;
						}
						byte[] picture = new byte[] { };
						if (pict != null)
						{
							using (var target = new MemoryStream())
							{
								await pict.CopyToAsync(target);
								picture = target.ToArray();
							}
						}
						else if (!del)
						{
							picture = (await grantService.GetElement(new Grant { Id = ElementId })).Picture;
						}

						DateTime DateStart = DateTime.ParseExact(datestart, "yyyy-M-dd", null);
						DateTime DateFinish = DateTime.ParseExact(datefinish, "yyyy-M-dd", null);
						DateTime DateSr = DateTime.ParseExact(datesr, "yyyy-M-dd", null);

						if (DateStart <= DateFinish && DateSr <= DateStart)
						{
							// Редактирование и отображение элемента
							await grantService.Update(new Grant { Id = ElementId,
								GrantName = name,
								GrantText = text,
								GrantSource = source,
								GrantDescription = desc,
								GrantStatus = status,
								GrantDeadline = DateSr,
								GrantDeadlineStart = DateStart,
								GrantDeadlineFinish = DateFinish,
								BlockId = BlockId,
								Picture = picture
							});
							Response.Redirect($"/grant/index?" +
								$"Id={ElementId}" +
								$"&BlockId={BlockId}");
						}
						else if (DateStart > DateFinish)
						{
							_notyf.Error("Дата начала должна быть раньше даты окончания.");
							Response.Redirect($"/grant/updgrant?" +
								$"Id={ElementId}" +
								$"&BlockId={BlockId}");
						}
						else if (DateSr >= DateStart)
						{
							_notyf.Error("Срок подачи заявки должен быть раньше срока начала выполнения.");
							Response.Redirect($"/grant/updgrant?" +
								$"Id={ElementId}" +
								$"&BlockId={BlockId}");
						}
						else
						{
							_notyf.Error("Ошибка в указании дат.");
							Response.Redirect($"/grant/updgrant?" +
								$"Id={ElementId}" +
								$"&BlockId={BlockId}");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/grant/updgrant?" +
								$"Id={id}" +
								$"&BlockId={blockId}");
					}
				}
				else
				{
					_notyf.Error("Ошибка");
					Response.Redirect("javascript: history.go(-1)");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/grant/updgrant?" +
						$"Id={id}" +
						$"&BlockId={blockId}");
			}
		}

		// Прикрепление участника к гранту
		[HttpGet]
		public async Task<IActionResult> AddPart(string grantId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Grant find_grant = await grantService.GetElement(new Grant { Id = Convert.ToInt32(grantId) });
			ViewBag.Grant = find_grant;

			if (find_grant != null)
			{
				List<Participant> all_part = await participantService.GetFullList();
				List<Participant> grant_part = await grantService.GetParticipants(find_grant.Id);

				List<Participant> part_for_adds = new List<Participant>();

				foreach (Participant participant in all_part)
				{
					bool add = true;
					foreach (Participant gp in grant_part) 
					{
						if (gp.Id == participant.Id) add = false;
					}
					if (add) { part_for_adds.Add(participant); }
				}
				ViewBag.Participants = part_for_adds;
				return View();
			}
			else
			{
				_notyf.Error("Грант не найден");
				return Redirect("javascript: history.go(-1)");	
			}
		}
		[HttpPost]
		public async Task AddPart(string partId, string grantId) 
		{
			// Привязка участника к гранту и отображение гранта с участниками
			int PartId = Convert.ToInt32(partId);
			int GrantId = Convert.ToInt32(grantId);

			Participant find_part = await participantService.GetElement(new Participant { Id = PartId });
			Grant find_grant = await grantService.GetElement(new Grant { Id = GrantId });

			if (find_part != null && find_grant != null)
			{
				try
				{
					await grantService.GetParticipant(find_part, find_grant.Id);
					Response.Redirect($"/grant/index?Id={GrantId}&BlockId={find_grant.BlockId}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/grant/addpart?blockId=" + grantId);
				}
			}
			else
			{
				_notyf.Error("Выберите участника");
				Response.Redirect($"/grant/addpart?blockId=" + grantId);
			}
		}

		// Открепление участника
		[HttpGet]
		public async Task DelPart(string partId, string grantId)
		{
			// Открепление участника от гранта и отображение гранта с участниками
			int PartId = Convert.ToInt32(partId);
			int GrantId = Convert.ToInt32(grantId);

			Participant find_part = await participantService.GetElement(new Participant { Id = PartId });
			Grant find_grant = await grantService.GetElement(new Grant { Id = GrantId });

			if (find_part != null && find_grant != null)
			{
				try
				{
					await grantService.GetParticipant(find_part, find_grant.Id);
					Response.Redirect($"/grant/index?Id={GrantId}&BlockId={find_grant.BlockId}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/grant/addpart?blockId=" + grantId);
				}
			}
			else
			{
				_notyf.Error("Участник не найден");
				Response.Redirect($"/grant/addpart?blockId=" + grantId);
			}
		}

		// Удаление гранта
		[HttpGet]
		public async Task DeleteGrant(string grantId)
		{
			if (!string.IsNullOrEmpty(grantId))
			{
				Grant find_grant = await grantService.GetElement(new Grant { Id = Convert.ToInt32(grantId) });
				if (find_grant != null)
				{
					try
					{
						Grant grant = await grantService.GetElement(new Grant { Id = find_grant.Id });
						await grantService.Delete(new Grant { Id = find_grant.Id });
						Response.Redirect($"/block/index?Id=" + grant.BlockId.ToString());
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/grant/index?Id=" + grantId);
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/grant/index?Id=" + grantId);
				}
			}
			else
			{
				_notyf.Error("Ошибка");
				Response.Redirect("javascript: history.go(-1)");
			}
		}
	}
}