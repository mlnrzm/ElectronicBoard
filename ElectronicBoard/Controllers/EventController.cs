using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Controllers
{
	public class EventController : Controller
	{
		private readonly ILogger<EventController> _logger;
		private readonly IEventService eventService;
		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IArticleService articleService;
		private readonly INotyfService _notyf;
		private readonly IStickerService stickerService;
		private readonly IFileService fileService;

		public EventController(ILogger<EventController> logger, INotyfService notyf, 
			IEventService _eventService, IBoardService _boardService, 
			IBlockService _blockService, IArticleService _articleService, IStickerService _stickerService, IFileService _fileService)
		{
			_logger = logger;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			eventService = _eventService;
			articleService = _articleService;
			stickerService = _stickerService;
			fileService = _fileService;
		}

		// Отображение страницы с мероприятием
		public async Task<IActionResult> Index(string eventId, string blockId)
		{
			int EventId = Convert.ToInt32(eventId);
			int BlockId = Convert.ToInt32(blockId);

			Event find_event = await eventService.GetElement(new Event { Id = EventId });

			// Конвертация изображения
			if (find_event.Picture.Length > 0) 
			{
				ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_event.Picture);
			}

			// Стикеры
			List<Sticker> stickers = await stickerService.GetFilteredList("event", find_event.Id);
			ViewBag.Stickers = stickers;

			// Файлы
			List<File> files = await fileService.GetFilteredList("event", EventId);
			ViewBag.Files = files;

			List<Article> added_articles = new List<Article>();
			foreach (var b in await articleService.GetFilteredList(EventId)) { added_articles.Add(b); }
			ViewBag.Articles = added_articles;

			// Блок, на котором находится элемент
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });
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
			return View(find_event);
		}

		// Добавление нового мероприятия
		[HttpGet]
		public IActionResult AddNewEvent(string blockId)
		{
			// Передача id блока, на котором будет находиться мероприятие
			ViewData["blockId"] = blockId;
			return View();
		}
		[HttpPost]
		public async Task AddNewEvent(string blockId, string name, string text, string place, 
			string datestart, string datestartcolor, string datefinish, string datefinishcolor,
			string datefinisharticle, string datefinisharticlecolor, IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
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

					int BlockId = Convert.ToInt32(blockId);

					DateTime DateStart = DateTime.ParseExact(datestart, "yyyy-M-dd", null);
					DateTime DateFinish = DateTime.ParseExact(datefinish, "yyyy-M-dd", null);
					DateTime DateFinishArticle = DateTime.ParseExact(datefinisharticle, "yyyy-M-dd", null);

					if (DateStart <= DateFinish && DateFinishArticle <= DateFinish)
					{
						// Добавление мероприятия
						await eventService.Insert(new Event { 
							EventName = name, 
							EventText = text, 
							EventPlace = place,

							Picture = picture,

							EventDateStart = DateStart,
							EventDateFinish = DateFinish,
							EventDateFinishArticle = DateFinishArticle,
							
							EventStartColor = datestartcolor,
							EventFinishColor = datefinishcolor,
							EventFinishArticleColor = datefinisharticlecolor
						});
						Event new_event = await eventService.GetElement(new Event
						{
							EventName = name
						});

						// Добавление мероприятия в блок и отображение мероприятия
						await blockService.AddOrRemoveElement(new_event, BlockId);
						Response.Redirect($"/event/index?" +
							$"eventId={new_event.Id}" +
							$"&blockId={BlockId}");
					}
					else if (DateStart > DateFinish)
					{
						_notyf.Error("Дата начала должна быть раньше даты окончания.");
						Response.Redirect($"/event/addnewevent?blockId=" + blockId);
					}
					else if (DateFinishArticle <= DateFinish)
					{
						_notyf.Error("Дата окончания приёма статей должна быть раньше, чем окончание мероприятия.");
						Response.Redirect($"/event/addnewevent?blockId=" + blockId);
					}
					else
					{
						_notyf.Error("Ошибка в указании дат.");
						Response.Redirect($"/event/addnewevent?blockId=" + blockId);
					}
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/event/addnewevent?blockId=" + blockId);
				}				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/event/addnewevent?blockId=" + blockId);
			}
		}

		// Добавление существующего мероприятия в блок
		[HttpGet]
		public async Task<IActionResult> AddEvent(string blockId)
		{
			int BlockId = Convert.ToInt32(blockId);
			// Передача id блока, на котором будет находиться мероприятие
			ViewData["blockId"] = blockId;

			// Передача мероприятий, которые можно добавить в блок (id - значения)
			List<Event> all_events = await eventService.GetFullList();
			List<Event> block_events = await eventService.GetFilteredList(null, BlockId);

			List<Event> events_for_adds = new List<Event>();
			foreach (var ev in all_events) 
			{
				bool add = true;
				foreach (var block_ev in block_events) 
				{
					if (ev.Id == block_ev.Id) add = false;
				}
				if (add) { events_for_adds.Add(ev); }
			}
			ViewBag.Events = events_for_adds;

			return View();
		}
		[HttpPost]
		public async Task AddEvent(string blockId, string eventId)
		{
			// Привязка мероприятия к блоку и отображение блока с мероприятиями
			int EventId = Convert.ToInt32(eventId);
			int BlockId = Convert.ToInt32(blockId);

			Event find_event = await eventService.GetElement(new Event { Id = EventId });
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });

			if (find_event != null && find_block != null)
			{
				try
				{
					await blockService.AddOrRemoveElement(find_event, find_block.Id);
					Response.Redirect($"/block/index?Id=" + blockId);
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/event/addevent?blockId=" + blockId);
				}
			}
			else 
			{
				_notyf.Error("Выберите мероприятие");
				Response.Redirect($"/event/addevent?blockId=" + blockId);
			}
		}

		// Редактирование мероприятия
		[HttpGet]
		public async Task<IActionResult> UpdEvent(string eventId, string blockId)
		{
			int EventId = Convert.ToInt32(eventId);
			int BlockId = Convert.ToInt32(blockId);

			Event find_event = await eventService.GetElement(new Event { Id = EventId });

			// Блок, на котором находится элемент
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });
			ViewData["blockId"] = find_block.Id;

			return View(find_event);
		}
		[HttpPost]
		public async Task UpdEvent(string id, string blockId, string name, string text, string place,
			string datestart, string datestartcolor, string datefinish, string datefinishcolor,
			string datefinisharticle, string datefinisharticlecolor, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(place)
				&& !string.IsNullOrEmpty(datestart) && !string.IsNullOrEmpty(datestartcolor) 
				&& !string.IsNullOrEmpty(datefinish) && !string.IsNullOrEmpty(datefinishcolor)
				&& !string.IsNullOrEmpty(datefinisharticle) && !string.IsNullOrEmpty(datefinisharticlecolor))
			{
				try
				{
					int Id = Convert.ToInt32(id);
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
						picture = (await eventService.GetElement(new Event { Id = Id })).Picture;
					}

					DateTime DateStart = DateTime.ParseExact(datestart, "yyyy-M-dd", null);
					DateTime DateFinish = DateTime.ParseExact(datefinish, "yyyy-M-dd", null);
					DateTime DateFinishArticle = DateTime.ParseExact(datefinisharticle, "yyyy-M-dd", null);

					if (DateStart <= DateFinish && DateFinishArticle <= DateFinish)
					{
						// Редактирование мероприятия
						await eventService.Update(new Event
						{
							Id = Id,
							EventName = name,
							EventText = text,
							EventPlace = place,

							Picture = picture,

							EventDateStart = DateStart,
							EventDateFinish = DateFinish,
							EventDateFinishArticle = DateFinishArticle,

							EventStartColor = datestartcolor,
							EventFinishColor = datefinishcolor,
							EventFinishArticleColor = datefinisharticlecolor

						});
						Event this_event = await eventService.GetElement(new Event
						{
							EventName = name
						});

						// Отображение мероприятия
						Response.Redirect($"/event/index?" +
							$"eventId={this_event.Id}" +
							$"&blockId={BlockId}");
					}
					else if (DateStart > DateFinish)
					{
						_notyf.Error("Дата начала должна быть раньше даты окончания.");
						Response.Redirect($"/event/updevent?" +
						$"Id={id}" +
						$"EventName={name}");
					}
                    else if (DateFinishArticle <= DateFinish)
                    {
						_notyf.Error("Дата окончания приёма статей должна быть раньше, чем окончание мероприятия.");
						Response.Redirect($"/event/updevent?" +
						$"Id={id}" +
						$"EventName={name}");
					}
					else 
					{
						_notyf.Error("Ошибка в указании дат.");
						Response.Redirect($"/event/updevent?" +
						$"Id={id}" +
						$"EventName={name}");
					}
                }
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/event/updevent?" +
						$"Id={id}" +
						$"EventName={name}");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/event/updevent?" +
							$"Id={id}" +
							$"EventName={name}");
			}
		}

		// Удаление мероприятия из блока
		[HttpGet]
		public async Task DeleteEventBlock(string blockId, string eventId)
		{
			if (!string.IsNullOrEmpty(eventId) && !string.IsNullOrEmpty(blockId))
			{
				int block_id;
				int event_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_eventId = int.TryParse(eventId, out event_id);
				if (isNumeric_blockId && isNumeric_eventId)
				{
					try
					{
						Event ev = await eventService.GetElement(new Event { Id = event_id });
						Block bl = await blockService.GetElement(new Block { Id = block_id });
						if (ev != null && bl != null) 
						{
							await blockService.AddOrRemoveElement(ev, block_id);
							Response.Redirect($"/block/index?Id=" + block_id);
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/event/index?eventId=" + eventId + "&blockId=" + blockId);
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/event/index?eventId=" + eventId + "&blockId=" + blockId);
				}
			}
		}

		// Удаление мероприятия из системы
		[HttpGet]
		public async Task DeleteEvent(string blockId, string eventId)
		{
			if (!string.IsNullOrEmpty(eventId) && !string.IsNullOrEmpty(blockId))
			{
				int block_id;
				int event_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_eventId = int.TryParse(eventId, out event_id);
				if (isNumeric_blockId && isNumeric_eventId)
				{
					try
					{
						Event ev = await eventService.GetElement(new Event { Id = event_id });
						Block bl = await blockService.GetElement(new Block { Id = block_id });
						if (ev != null && bl != null)
						{
							await blockService.AddOrRemoveElement(ev, block_id);
							await eventService.Delete(new Event { Id = event_id });
							Response.Redirect($"/block/index?Id=" + block_id);
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/event/index?eventId=" + eventId + "&blockId=" + blockId);
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/event/index?eventId=" + eventId + "&blockId=" + blockId);
				}
			}
		}
	}
}