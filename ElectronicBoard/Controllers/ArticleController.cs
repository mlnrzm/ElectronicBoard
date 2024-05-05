using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Controllers
{
	public class ArticleController : Controller
	{
		private readonly ILogger<ArticleController> _logger;
		private readonly INotyfService _notyf;

		private readonly IEventService eventService;
		private readonly IArticleService articleService;
		private readonly IAggregatorService aggregatorService;
		private readonly IAuthorService authorService;
		private readonly IFileService fileService;

		private readonly IBlockService blockService;
		private readonly IBoardService boardService;


		public ArticleController(ILogger<ArticleController> logger, INotyfService notyf, 
			IEventService _eventService, IArticleService _articleService, IAuthorService _authorService, 
			IBlockService _blockService, IBoardService _boardService, IAggregatorService _aggregatorService, IFileService _fileService)
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
		}

		// Отображение страницы с информацией о статье
		public async Task<IActionResult> Index(string blockId, string articleId)
		{
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			Article find_article = await articleService.GetElement(new Article { Id = ArticleId });

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
			Event find_event = await eventService.GetElement(new Event { Id = find_article.EventId });
			ViewBag.Event = find_event;

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

			return View(find_article);
		}

		// Добавление/создание статьи
		[HttpGet]
		public IActionResult AddArticle(string blockId, string eventId)
		{
			// Передача id блока, на котором будет находиться мероприятие со статьёй
			ViewData["blockId"] = blockId;

			// Передача id мероприятия, на котором будет находиться статья
			ViewData["eventId"] = eventId;

			return View();
		}
		[HttpPost]
		public async Task AddArticle(string blockId, string eventId, 
			string name, string text, string place, string words, string annotation, string category,
			IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(eventId) 
				&& !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text) 
				&& !string.IsNullOrEmpty(place) && !string.IsNullOrEmpty(words) && !string.IsNullOrEmpty(annotation) && !string.IsNullOrEmpty(category) )
			{
				try
				{
					int BlockId = Convert.ToInt32(blockId);
					int EventId = Convert.ToInt32(eventId);

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
						ArticleStatus = status, ArticleAnnotation = annotation, EventId = EventId, Picture = new byte[] { } });
					Article new_article = await articleService.GetElement(new Article
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
						$"&articleId={new_article.Id}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/addarticle?blockId=" + blockId + "&eventId=" + eventId);
				}
				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/article/addarticle?blockId=" + blockId + "&eventId=" + eventId);
			}
		}

		// Редактирование статьи
		[HttpGet]
		public async Task<IActionResult> UpdArticle(string id, string blockId, string eventId)
		{
			// Передача id статьи
			int art_id = Convert.ToInt32(id);
			Article this_art = await articleService.GetElement(new Article { Id = art_id });

			// Передача id блока, на котором будет находиться мероприятие со статьёй
			ViewData["blockId"] = blockId;

			// Передача id мероприятия, на котором будет находиться статья
			ViewData["eventId"] = eventId;

			return View(this_art);
		}
		[HttpPost]
		public async Task UpdArticle(string id, string blockId, string eventId, 
			string name, string text, string place, 
			string words, string annotation, string category, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(eventId) && 
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
						picture = (await articleService.GetElement(new Article { Id = Id })).Picture;
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
					Article _article = await articleService.GetElement(new Article
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
					Response.Redirect($"/article/updarticle?id=" + id + "&blockId=" + blockId + "&eventId=" + eventId);
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/article/updarticle?id=" + id + "&blockId=" + blockId + "&eventId=" + eventId);
			}
		}

		// Удаление статьи
		[HttpGet]
		public void DeleteArticle(string id, string blockId, string eventId)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(eventId) && !string.IsNullOrEmpty(blockId))
			{
				int _id;
				int block_id;
				int event_id;

				bool isNumeric_Id = int.TryParse(id, out _id);
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_eventId = int.TryParse(eventId, out event_id);

				if (isNumeric_blockId && isNumeric_Id && isNumeric_eventId)
				{
					try
					{
						articleService.Delete(new Article { Id = _id });
						Response.Redirect($"/event/index?eventId={eventId}&blockId={blockId}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/article/index?" +
							$"blockId={blockId}" +
							$"&articleId={id}" +
							$"&eventId={eventId}");
					}
				}
				else
				{
					_notyf.Error("Статья не найдена");
					Response.Redirect($"/article/index?" +
						$"blockId={blockId}" +
						$"&articleId={id}" +
						$"&eventId={eventId}");
				}
			}
		}
		
		// АГРЕГАТОРЫ СТАТЬИ //

		// Добавление существующего агрегатора
		[HttpGet]
		public async Task<IActionResult> AddAggregator(string articleId, string blockId)
		{
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			// Передача id статьи и блока
			ViewData["articleId"] = ArticleId;
			ViewData["blockId"] = BlockId;

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
		[HttpPost]
		public async Task AddAggregator(string blockId, string articleId, string aggregatorId)
		{
			// Привязка агрегатора к статье и отображение статьи с агрегаторами
			int AggregatorId = Convert.ToInt32(aggregatorId);
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			Aggregator find_aggregator = await aggregatorService.GetElement(new Aggregator { Id = AggregatorId });
			Article find_article = await articleService.GetElement(new Article { Id = ArticleId });
			Event find_event = await eventService.GetElement(new Event { Id = find_article.EventId });

			if (find_aggregator != null && find_article != null)
			{
				try
				{
					await articleService.GetAggregator(find_aggregator, find_article.Id);
					Response.Redirect($"/article/index?blockId=" + BlockId + "&eventId=" + find_event.Id + "&articleId=" + find_article.Id);
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/addaggregator?" +
						$"blockId=" + blockId 
						+ "&articleId=" + articleId);
				}
			}
			else
			{
				_notyf.Error("Выберите агрегатора");
				Response.Redirect($"/article/addaggregator?" +
					$"blockId=" + blockId
					+ "&articleId=" + articleId);
			}
		}

		// Удаление агрегатора из статьи
		[HttpGet]
		public async Task DeleteAggregatorArticle(string blockId, string articleId, string aggregatorId)
		{
			if (!string.IsNullOrEmpty(articleId) && !string.IsNullOrEmpty(aggregatorId))
			{
				// найти статью
				int block_id;
				int article_id;
				int aggregator_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_articleId = int.TryParse(articleId, out article_id);
				bool isNumeric_aggregatorId = int.TryParse(aggregatorId, out aggregator_id);
				if (isNumeric_articleId && isNumeric_aggregatorId && isNumeric_blockId)
				{
					Block block = await blockService.GetElement(new Block { Id = block_id });
					Aggregator agg = await aggregatorService.GetElement(new Aggregator { Id = aggregator_id });
					Article art = await articleService.GetElement(new Article { Id = article_id });
					try
					{
						if (agg != null && art != null && block != null)
						{
							await articleService.GetAggregator(agg, art.Id);
							Response.Redirect($"/article/index?blockId=" + block_id + "&articleId=" + articleId);
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/article/index?blockId=" + block_id + "&articleId=" + articleId);
					}
				}
				else
				{
					_notyf.Error("Ошибка");
					Response.Redirect($"/article/index?blockId=" + block_id + "&articleId=" + articleId);
				}
			}
		}

		// Удаление агрегатора 
		[HttpGet]
		public async Task DeleteAggregator(string blockId, string articleId, string aggregatorId)
		{
			// Привязка агрегатора к статье и отображение статьи с агрегаторами
			int AggregatorId = Convert.ToInt32(aggregatorId);
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			Aggregator find_aggregator = await aggregatorService.GetElement(new Aggregator { Id = AggregatorId });
			Article find_article = await articleService.GetElement(new Article { Id = ArticleId });
			Event find_event = await eventService.GetElement(new Event { Id = find_article.EventId });

			if (find_aggregator != null && find_article != null)
			{
				try
				{
					await aggregatorService.Delete(find_aggregator);
					Response.Redirect($"/article/index?blockId=" + BlockId + "&eventId=" + find_event.Id + "&articleId=" + find_article.Id);
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/index?blockId=" + BlockId + "&eventId=" + find_event.Id + "&articleId=" + find_article.Id);
				}
			}
			else
			{
				_notyf.Error("Агрегатор не найден");
				Response.Redirect($"/article/index?blockId=" + BlockId + "&eventId=" + find_event.Id + "&articleId=" + find_article.Id);
			}
		}

		// Добавление нового агрегатора
		[HttpGet]
		public IActionResult AddNewAggregator(string articleId, string blockId)
		{
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			// Передача id статьи и блока
			ViewData["articleId"] = ArticleId;
			ViewData["blockId"] = BlockId;

			return View();
		}
		[HttpPost]
		public async Task AddNewAggregator(string blockId, string articleId, string name)
		{
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			Article find_article = await articleService.GetElement(new Article { Id = ArticleId });
			Event find_event = await eventService.GetElement(new Event { Id = find_article.EventId });

			if (find_event != null && find_article != null && !string.IsNullOrEmpty(name))
			{
				try
				{
					await aggregatorService.Insert(new Aggregator { AggregatorName = name });
					Aggregator new_agg = await aggregatorService.GetElement(new Aggregator { AggregatorName = name });
					await articleService.GetAggregator(new_agg, find_article.Id);
					Response.Redirect($"/article/index?blockId=" + BlockId + "&eventId=" + find_event.Id + "&articleId=" + find_article.Id);
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/article/addnewaggregator?" +
						$"blockId=" + blockId
						+ "&articleId=" + articleId);
				}
			}
			else
			{
				_notyf.Error("Ошибка");
				Response.Redirect($"/article/addnewaggregator?" +
					$"blockId=" + blockId
					+ "&articleId=" + articleId);
			}
		}
	}
}
