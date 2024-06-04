using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Controllers
{
	/// <summary>
	/// Контроллер, обрабатывающий запросы касающиеся статей
	/// </summary>
	[Authorize]
	public class ArticleController : Controller
	{
		private readonly ILogger<ArticleController> _logger;
		private readonly INotyfService _notyf;
		private readonly IdnMapping idn;

		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly IParticipantService participantService;

		private readonly IEventService eventService;
		private readonly IArticleService articleService;
		private readonly IAggregatorService aggregatorService;
		private readonly IAuthorService authorService;
		private readonly IFileService fileService;

		private readonly IBlockService blockService;
		private readonly IBoardService boardService;


		public ArticleController(ILogger<ArticleController> logger, INotyfService notyf, 
			IEventService _eventService, IArticleService _articleService, IAuthorService _authorService, 
			IBlockService _blockService, IBoardService _boardService, IAggregatorService _aggregatorService, IFileService _fileService,
			UserManager<IdentityUser<int>> userManager, IParticipantService _participantService)
		{
			_logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			articleService = _articleService;
			eventService = _eventService;
			boardService = _boardService;
			aggregatorService = _aggregatorService;
			authorService = _authorService;
			fileService = _fileService;
			idn = new IdnMapping();
			_userManager = userManager;
			participantService = _participantService;
		}

		/// <summary>
		/// Метод для отображения страницы с информацией о статье
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="articleId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Index(string blockId, string articleId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				if (activeUser != null)
				{
					ViewBag.ActivePart = activeUser;
					List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
					ViewBag.ActiveBoards = activeBoards;
				}

				int ArticleId = Convert.ToInt32(articleId);
				int BlockId = Convert.ToInt32(blockId);

				Article? find_article = await articleService.GetElement(new Article { Id = ArticleId });

				if (find_article != null)
				{
					// Конвертация изображения
					if (find_article.Picture.Length > 0)
					{
						ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_article.Picture);
					}

					// Агрегаторы статьи
					List<Aggregator> aggregators = await aggregatorService.GetFilteredList(find_article.Id);
					ViewBag.Aggregators = aggregators;

					// Авторы статьи
					List<Author> authors = await authorService.GetFilteredList(find_article.Id);
					ViewBag.Authors = authors;

					// Файлы статьи
					List<File> files = await fileService.GetFilteredList("article", ArticleId);
					ViewBag.Files = files;

					// Мероприятие, в котором находится статья
					Event? find_event = await eventService.GetElement(new Event { Id = find_article.EventId });
					ViewBag.Event = find_event;

					// Блок, на котором находится элемент
					Block? find_block = await blockService.GetElement(new Block { Id = BlockId });

					if (find_block != null)
					{
						ViewBag.Block = find_block;

						// Доска, на которой находится блок
						Board? board = await boardService.GetElement(new Board { Id = find_block.BoardId });
						List<Block> added_blocks = new List<Block>();

						if (board != null)
						{
							foreach (var b in await blockService.GetFilteredList(new Block { BoardId = board.Id })) { added_blocks.Add(b); }
							ViewBag.Board = new Board
							{
								Id = board.Id,
								BoardName = board.BoardName,
								BoardThematics = board.BoardThematics,
								Blocks = added_blocks
							};
						}
					}
				}
				return View(find_article);
			}
			catch (Exception ex) 
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}

		/// <summary>
		/// Метод для отображения страницы добавления статьи
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="eventId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddArticle(string blockId, string eventId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				var block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				var _event = await eventService.GetElement(new Event { Id = Convert.ToInt32(eventId) });

				if (block != null && _event != null)
				{
					// Передача id блока, на котором будет находиться мероприятие со статьёй
					ViewData["blockId"] = blockId;
					// Передача id мероприятия, на котором будет находиться статья
					ViewData["eventId"] = eventId;
				}
				else 
				{
					throw new Exception("Ошибка");
				}
				return View();
			}
			catch(Exception ex) 
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task AddArticle(string blockId, string eventId, 
			string name, string text, string place, string words, string annotation, string category,
			IFormFile pict)
		{
			var block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
			var _event = await eventService.GetElement(new Event { Id = Convert.ToInt32(eventId) });

			if (block != null && _event != null 
				&& !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) 
				&& !string.IsNullOrEmpty(place) && !string.IsNullOrEmpty(words) && !string.IsNullOrEmpty(annotation) && !string.IsNullOrEmpty(category) )
			{
				try
				{
					// Изображение
					byte[] picture = new byte[] { };
					if (pict != null)
					{
						using (var target = new MemoryStream())
						{
							pict.CopyTo(target);
							picture = target.ToArray();
						}
					}

					// Статус статьи
					StatusArticle status = StatusArticle.Планируется;
					switch (category)
					{
						case "0":
							status = StatusArticle.Планируется;
							break;
						case "1":
							status = StatusArticle.Отправлена;
							break;
						case "2":
							status = StatusArticle.Проверка;
							break;
						case "3":
							status = StatusArticle.Отклонена;
							break;
						case "4":
							status = StatusArticle.Сборник;
							break;
						case "5":
							status = StatusArticle.Опубликована;
							break;
						default:
							throw new Exception("Укажите статус статьи");
					}

					// Добавление и отображение статьи
					await articleService.Insert(new Article { ArticleName = name, ArticleText = text, ArticlePlaceOfPublication = place, ArticleKeyWords = words, 
						ArticleStatus = status, ArticleAnnotation = annotation, EventId = _event.Id, Picture = picture });
					Article? new_article = await articleService.GetElement(new Article
					{
						ArticleName = name,
						ArticleText = text,
						ArticlePlaceOfPublication = place,
						ArticleKeyWords = words,
						ArticleStatus = status,
						ArticleAnnotation = annotation,
						EventId = _event.Id,
						Picture = picture
					});

					Response.Redirect($"/article/index?" +
						$"blockId={block.Id}" +
						$"&articleId={new_article.Id}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/addarticle?blockId=" + idn.GetAscii(blockId) + "&eventId=" + idn.GetAscii(eventId));
				}
				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/article/addarticle?blockId=" + idn.GetAscii(blockId) + "&eventId=" + idn.GetAscii(eventId));
			}
		}

		/// <summary>
		/// Метод для отображения страницы редактирования статьи
		/// </summary>
		/// <param name="id"></param>
		/// <param name="blockId"></param>
		/// <param name="eventId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> UpdArticle(string id, string blockId, string eventId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				// Передача id статьи
				Article? this_art = await articleService.GetElement(new Article { Id = Convert.ToInt32(id) });
				var block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				var _event = await eventService.GetElement(new Event { Id = Convert.ToInt32(eventId) });

				if (block != null && _event != null && this_art != null)
				{
					// Передача id блока, на котором будет находиться мероприятие со статьёй
					ViewData["blockId"] = blockId;
					// Передача id мероприятия, на котором будет находиться статья
					ViewData["eventId"] = eventId;
				}
				else
				{
					throw new Exception("Статья не найдена");
				}
				return View(this_art);
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task UpdArticle(string id, string blockId, string eventId, 
			string name, string text, string place, 
			string words, string annotation, string category, IFormFile pict, string delpic)
		{
			var block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
			var _event = await eventService.GetElement(new Event { Id = Convert.ToInt32(eventId) });
			var art = await articleService.GetElement(new Article { Id = Convert.ToInt32(id) });

			if (block != null && _event != null && art != null &&
				!string.IsNullOrEmpty(place) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) &&
				!string.IsNullOrEmpty(words) && !string.IsNullOrEmpty(annotation) && !string.IsNullOrEmpty(category))
			{
				try
				{
					int Id = Convert.ToInt32(id);
					int BlockId = Convert.ToInt32(blockId);
					int EventId = Convert.ToInt32(eventId);

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
							pict.CopyTo(target);
							picture = target.ToArray();
						}
					}
					else if (!del)
					{
						var article = await articleService.GetElement(new Article { Id = Id });
						if (article != null) picture = article.Picture;
					}

					// Статус статьи 
					StatusArticle status = StatusArticle.Планируется;
					switch (category)
					{
						case "0":
							status = StatusArticle.Планируется;
							break;
						case "1":
							status = StatusArticle.Отправлена;
							break;
						case "2":
							status = StatusArticle.Проверка;
							break;
						case "3":
							status = StatusArticle.Отклонена;
							break;
						case "4":
							status = StatusArticle.Сборник;
							break;
						case "5":
							status = StatusArticle.Опубликована;
							break;
						default:
							throw new Exception("Укажите статус статьи");
					}

					// Редактирование и отображение статьи
					await articleService.Update(new Article
					{
						Id = Id,
						ArticleName = name,
						ArticleText = text,
						ArticlePlaceOfPublication = place,
						ArticleKeyWords = words,
						ArticleStatus = status,
						ArticleAnnotation = annotation,
						EventId = EventId,
						Picture = picture
					});
					Article? _article = await articleService.GetElement(new Article
					{
						ArticleName = name,
						ArticleText = text,
						ArticlePlaceOfPublication = place,
						ArticleKeyWords = words,
						ArticleStatus = status,
						ArticleAnnotation = annotation,
						EventId = EventId,
						Picture = picture
					});

					Response.Redirect($"/article/index?" +
						$"blockId={BlockId}" +
						$"&articleId={_article.Id}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/updarticle?id=" + idn.GetAscii(id.ToString()) + "&blockId=" + idn.GetAscii(blockId.ToString()) + "&eventId=" + idn.GetAscii(eventId));
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/article/updarticle?id=" + idn.GetAscii(id) + "&blockId=" + idn.GetAscii(blockId) + "&eventId=" + idn.GetAscii(eventId));
			}
		}

		/// <summary>
		/// Метод для удаления статьи
		/// </summary>
		/// <param name="id"></param>
		/// <param name="blockId"></param>
		/// <param name="eventId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task DeleteArticle(string id, string blockId, string eventId)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(eventId) && !string.IsNullOrEmpty(blockId))
			{
				var block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				var _event = await eventService.GetElement(new Event { Id = Convert.ToInt32(eventId) });
				var art = await articleService.GetElement(new Article { Id = Convert.ToInt32(id) });

				if (block != null && _event != null && art != null)
				{
					try
					{
						await articleService.Delete(new Article { Id = art.Id });
						Response.Redirect($"/event/index?eventId={idn.GetAscii(eventId)}&blockId={idn.GetAscii(blockId)}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/article/index?" +
							$"blockId={idn.GetAscii(blockId)}" +
							$"&articleId={idn.GetAscii(id)}" +
							$"&eventId={idn.GetAscii(eventId)}");
					}
				}
				else
				{
					_notyf.Error("Статья не найдена");
					Response.Redirect($"/article/index?" +
						$"blockId={idn.GetAscii(blockId)}" +
						$"&articleId={idn.GetAscii(id)}" +
						$"&eventId={idn.GetAscii(eventId)}");
				}
			}
			else
			{
				Response.Redirect("javascript: history.go(-1)");
			}
		}

		/// <summary>
		/// Ниже представлены методы для работы с агрегаторами статьи
		/// </summary>

		/// <summary>
		/// Метод для отображения страницы добавления существующего агрегатора к статье
		/// </summary>
		/// <param name="articleId"></param>
		/// <param name="blockId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddAggregator(string articleId, string blockId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				int ArticleId = Convert.ToInt32(articleId);

				var block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				var art = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });

				if (block != null && art != null)
				{
					// Передача id статьи и блока
					ViewData["articleId"] = art.Id;
					ViewData["blockId"] = block.Id;

					// Передача агрегаторов, которых можно добавить к статье
					List<Aggregator> all_aggs = await aggregatorService.GetFullList();
					List<Aggregator> article_aggs = await aggregatorService.GetFilteredList(ArticleId);

					List<Aggregator> aggs_for_adds = new List<Aggregator>();
					foreach (var agg in all_aggs)
					{
						bool add = true;
						foreach (var art_agg in article_aggs)
						{
							if (agg.Id == art_agg.Id) add = false;
						}
						if (add) { aggs_for_adds.Add(agg); }
					}
					ViewBag.Aggregators = aggs_for_adds;

					return View();
				}
				else
				{
					_notyf.Error("Статья не найдена");
					return Redirect("javascript: history.go(-1)");
				}
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}

		}
		[HttpPost]
		public async Task AddAggregator(string blockId, string articleId, string aggregatorId)
		{
			// Привязка агрегатора к статье и отображение статьи с агрегаторами
			int AggregatorId = Convert.ToInt32(aggregatorId);
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			Aggregator? find_aggregator = await aggregatorService.GetElement(new Aggregator { Id = AggregatorId });
			Article? find_article = await articleService.GetElement(new Article { Id = ArticleId });
			Event? find_event = await eventService.GetElement(new Event { Id = find_article.EventId });

			if (find_aggregator != null && find_article != null && find_event != null)
			{
				try
				{
					await articleService.GetAggregator(find_aggregator, find_article.Id);
					Response.Redirect($"/article/index?blockId=" + idn.GetAscii(BlockId.ToString()) + "&eventId=" + idn.GetAscii(find_event.Id.ToString()) + "&articleId=" + idn.GetAscii(find_article.Id.ToString()));
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/addaggregator?" +
						$"blockId=" + idn.GetAscii(blockId) 
						+ "&articleId=" + idn.GetAscii(articleId));
				}
			}
			else
			{
				_notyf.Error("Выберите агрегатора");
				Response.Redirect($"/article/addaggregator?" +
					$"blockId=" + idn.GetAscii(blockId)
					+ "&articleId=" + idn.GetAscii(articleId));
			}
		}

		/// <summary>
		/// Метод для удаления агрегатора из статьи
		/// </summary>
		/// <param name="aggregatorId"></param>
		/// <param name="blockId"></param>
		/// <param name="articleId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task DeleteAggregatorArticle(string blockId, string articleId, string aggregatorId)
		{
			if (!string.IsNullOrEmpty(articleId) && !string.IsNullOrEmpty(aggregatorId) && !string.IsNullOrEmpty(blockId))
			{
				Aggregator? find_aggregator = await aggregatorService.GetElement(new Aggregator { Id = Convert.ToInt32(aggregatorId) });
				Article? find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
				Block? find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

				if (find_aggregator != null && find_article != null && find_block != null)
				{
					try
					{
						await articleService.GetAggregator(find_aggregator, find_article.Id);
						Response.Redirect($"/article/index?blockId=" + find_block.Id + "&articleId=" + find_article.Id);
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/article/index?blockId=" + find_block.Id + "&articleId=" + find_article.Id);
					}
				}
				else
				{
					_notyf.Error("Ошибка");
					Response.Redirect("javascript: history.go(-1)");
				}
			}
		}

		/// <summary>
		/// Метод для удаления агрегатора
		/// </summary>
		/// <param name="aggregatorId"></param>
		/// <param name="blockId"></param>
		/// <param name="articleId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task DeleteAggregator(string blockId, string articleId, string aggregatorId)
		{
			try
			{
				int AggregatorId = Convert.ToInt32(aggregatorId);
				int ArticleId = Convert.ToInt32(articleId);
				int BlockId = Convert.ToInt32(blockId);

				Aggregator? find_aggregator = await aggregatorService.GetElement(new Aggregator { Id = AggregatorId });
				Article? find_article = await articleService.GetElement(new Article { Id = ArticleId });
				Event? find_event = await eventService.GetElement(new Event { Id = find_article.EventId });

				if (find_aggregator != null && find_article != null)
				{
					try
					{
						await aggregatorService.Delete(find_aggregator);
						Response.Redirect($"/article/index?blockId=" + idn.GetAscii(BlockId.ToString()) + "&eventId=" + idn.GetAscii(find_event.Id.ToString()) + "&articleId=" + idn.GetAscii(find_article.Id.ToString()));
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/article/index?blockId=" + idn.GetAscii(BlockId.ToString()) + "&eventId=" + idn.GetAscii(find_event.Id.ToString()) + "&articleId=" + idn.GetAscii(find_article.Id.ToString()));
					}
				}
				else
				{
					_notyf.Error("Агрегатор не найден");
					Response.Redirect($"/article/index?blockId=" + idn.GetAscii(BlockId.ToString()) + "&eventId=" + idn.GetAscii(find_event.Id.ToString()) + "&articleId=" + idn.GetAscii(articleId));
				}
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				Response.Redirect("javascript: history.go(-1)");
			}
		}

		/// <summary>
		/// Метод для отображения страницы добавления нового агрегатора
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="articleId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddNewAggregator(string articleId, string blockId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Article? find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
				Block find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

				if (find_article != null && find_block != null)
				{
					// Передача id статьи и блока
					ViewData["articleId"] = find_article.Id;
					ViewData["blockId"] = find_block.Id;
					return View();
				}
				else
				{
					_notyf.Error("Ошибка");
					return Redirect("javascript: history.go(-1)");
				}
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task AddNewAggregator(string blockId, string articleId, string name)
		{
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			Article? find_article = await articleService.GetElement(new Article { Id = ArticleId });
			Event? find_event = await eventService.GetElement(new Event { Id = find_article.EventId });

			if (find_event != null && find_article != null && !string.IsNullOrEmpty(name))
			{
				try
				{
					await aggregatorService.Insert(new Aggregator { AggregatorName = name });
					Aggregator? new_agg = await aggregatorService.GetElement(new Aggregator { AggregatorName = name });
					if (new_agg != null) await articleService.GetAggregator(new_agg, find_article.Id);
					Response.Redirect($"/article/index?blockId=" + idn.GetAscii(BlockId.ToString()) + "&eventId=" + idn.GetAscii(find_event.Id.ToString()) + "&articleId=" + idn.GetAscii(find_article.Id.ToString()));
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/addnewaggregator?" +
						$"blockId=" + idn.GetAscii(blockId.ToString())
						+ "&articleId=" + idn.GetAscii(articleId.ToString()));
				}
			}
			else
			{
				_notyf.Error("Ошибка");
				Response.Redirect($"/article/addnewaggregator?" +
					$"blockId=" + idn.GetAscii(blockId.ToString())
					+ "&articleId=" + idn.GetAscii(articleId.ToString()));
			}
		}
	}
}