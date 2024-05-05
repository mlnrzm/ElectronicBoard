using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Controllers
{
    public class ParticipantController : Controller
	{
		private readonly ILogger<ParticipantController> _logger;
		private readonly INotyfService _notyf;

		private readonly IParticipantService participantService;
		private readonly IStickerService stickerService;
		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IFileService fileService;

		private readonly IArticleService articleService;
		private readonly IAuthorService authorService;

		public ParticipantController(ILogger<ParticipantController> logger, 
			INotyfService notyf, IStickerService _stickerService, IParticipantService _participantService, 
			IBoardService _boardService, IBlockService _blockService, IFileService _fileService, 
			IArticleService _articleService, IAuthorService _authorService)
		{
			_logger = logger;
			participantService = _participantService;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			stickerService = _stickerService;
			fileService = _fileService;
			articleService = _articleService;
			authorService = _authorService;
		}

		// Редактировать профиль
		[HttpGet]
		public async Task<IActionResult> UpdProfile(Participant participant)
		{
			Participant find_element = await participantService.GetElement(new Participant
			{
				Id = participant.Id
			});
			return View(find_element);
		}
		[HttpPost]
		public async Task UpdProfile(string id,
			string name, string patents, string publics, string tasks, string interests, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(patents) &&
				!string.IsNullOrEmpty(publics) && !string.IsNullOrEmpty(tasks) && !string.IsNullOrEmpty(interests))
			{
				try
				{
					// ID элемента
					int ElementId = Convert.ToInt32(id);

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
						picture = (await participantService.GetElement(new Participant { Id = ElementId })).Picture;
					}

					// Редактирование и отображение профиля
					await participantService.Update(new Participant
					{
						Id = ElementId,
						Picture = picture,
						ParticipantFIO = name,
						ParticipantPatents = patents,
						ParticipantPublications = publics,
						ParticipantRating = 0,
						ParticipantTasks = tasks,
						ScientificInterests = interests
					});
					Response.Redirect($"/participant/profile?" +
						$"partId={ElementId}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/participant/updprofile?" +
						$"Id={id}");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/participant/updprofile?" +
					$"Id={id}");
			}
		}

		// Обновить рейтинг
		public async Task UpdRaiting(string partId)
		{
			int PartId = Convert.ToInt32(partId);
			Participant find_part = await participantService.GetElement(new Participant { Id = PartId });

			if (find_part != null)
			{
				// Метод обновления рейтинга участника
				await participantService.UpdRaiting(find_part.Id);
				find_part = await participantService.GetElement(new Participant { Id = PartId });
			}
			else
			{
				_notyf.Error("Участник не найден");
				Response.Redirect($"/board/index?" +
					$"&BoardName={"Общая доска"}");
			}
			Response.Redirect($"/participant/profile?" +
					$"&partId={partId}");
		}

		public async Task<IActionResult> Profile(string partId)
		{
			int PartId = Convert.ToInt32(partId);

			Participant find_part = await participantService.GetElement(new Participant { Id = PartId });

			if (find_part != null)
			{
				// Конвертация изображения
				if (find_part.Picture.Length > 0)
				{
					ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_part.Picture);
				}

				// Стикеры
				List<Sticker> stickers = await stickerService.GetFilteredList("participant", find_part.Id);
				ViewBag.Stickers = stickers;

				// Файлы
				List<File> files = await fileService.GetFilteredList("participant", find_part.Id);
				ViewBag.Files = files;

				// Статьи
				Author author = await authorService.GetElement(new Author { ParticipantId = find_part.Id });
				List<Article> articles = new List<Article>();
				if (author != null) { articles = await articleService.GetArticlesAuthor(author.Id); }
				ViewBag.Articles = articles;
			}
			else
			{
				_notyf.Error("Участник не найден");
				Response.Redirect($"/board/index?" +
					$"&BoardName={"Общая доска"}");
			}
			return View(find_part);
		}

		public async Task<IActionResult> Index(string partId, string blockId)
		{
			int PartId = Convert.ToInt32(partId);
			int BlockId = Convert.ToInt32(blockId);

			Participant find_part = await participantService.GetElement(new Participant { Id = PartId });
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });

			if (find_block != null && find_part != null)
			{
				ViewBag.Block = find_block;
				// Конвертация изображения
				if (find_part.Picture.Length > 0)
				{
					ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_part.Picture);
				}

				// Стикеры
				List<Sticker> stickers = await stickerService.GetFilteredList("participant", find_part.Id);
				ViewBag.Stickers = stickers;

				// Файлы
				List<File> files = await fileService.GetFilteredList("participant", find_part.Id);
				ViewBag.Files = files;

				// Статьи
				Author author = await authorService.GetElement(new Author { ParticipantId = find_part.Id });
				List<Article> articles = await articleService.GetArticlesAuthor(author.Id);
				ViewBag.Articles = articles;

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
			}
			else 
			{
				_notyf.Error("Участник не найден");
				Response.Redirect($"/block/index?" +
					$"Id={blockId}");
			}
			return View(find_part);
		}

		[HttpGet]
		public IActionResult Enter()
		{
			return View();
		}

		[HttpPost]
		public async Task Enter(string login, string password)
		{
			if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
			{
				// Искать участника в БД
				int count = (await participantService.GetFullList()).Count;
				Program.Participant = await participantService.Enter(login, password);

				// Вход в систему, отображение экрана с общей доской
				Program.Boards = await boardService.GetParticipantBoards(Program.Participant.Id);
				Board main_board = await boardService.GetElement(new Board
				{
					BoardName = "Общая доска",
					BoardThematics = "Общая"
				});
				Program.MainBoard = main_board;
				Response.Redirect($"/board/index?" +
					$"Id={main_board.Id}" +
					$"&BoardName={main_board.BoardName}" +
					$"&BoardThematics={main_board.BoardThematics}");
				return;
			}
			else
			{
				_notyf.Error("Введите логин и пароль");
				Response.Redirect($"/participant/enter");
				return;
			}
		}

		// Прикрепление участника к гранту
		[HttpGet]
		public async Task<IActionResult> AddPartBlock(string blockId)
		{
			int BlockId = Convert.ToInt32(blockId);
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });
			ViewBag.Block = find_block;

			if (find_block != null)
			{
				List<Participant> all_part = await participantService.GetFullList();
				List<Participant> block_part = await participantService.GetFilteredList(null, find_block.Id);

				List<Participant> part_for_adds = new List<Participant>();

				foreach (Participant participant in all_part)
				{
					bool add = true;
					foreach (Participant gp in block_part)
					{
						if (gp.Id == participant.Id) add = false;
					}
					if (add) { part_for_adds.Add(participant); }
				}
				ViewBag.Participants = part_for_adds;
			}
			else
			{
				_notyf.Error("Блок не найден");
				Response.Redirect($"/block/index?" +
					$"Id={blockId}");
			}
			return View();
		}
		[HttpPost]
		public async Task AddPartBlock(string partId, string blockId)
		{
			// Привязка участника к гранту и отображение гранта с участниками
			int PartId = Convert.ToInt32(partId);
			int BlockId = Convert.ToInt32(blockId);

			Participant find_part = await participantService.GetElement(new Participant { Id = PartId });
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });

			if (find_part != null && find_block != null)
			{
				try
				{
					await blockService.AddOrRemoveElement(find_part, find_block.Id);
					Response.Redirect($"/block/index?Id={find_block.Id}&BoardId={find_block.BoardId}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/participant/addpartblock?blockId=" + blockId);
				}
			}
			else
			{
				_notyf.Error("Выберите участника");
				Response.Redirect($"/participant/addpartblock?blockId=" + blockId);
			}
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

}
