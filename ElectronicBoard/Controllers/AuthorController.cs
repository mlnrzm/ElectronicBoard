using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicBoard.Controllers
{
	public class AuthorController : Controller
	{
		private readonly ILogger<AuthorController> _logger;
		private readonly INotyfService _notyf;

		private readonly IArticleService articleService;
		private readonly IAuthorService authorService;
		private readonly IParticipantService participantService;

		private readonly IBoardService boardService;
		private readonly IBlockService blockService;

		public AuthorController(ILogger<AuthorController> logger, INotyfService notyf, 
			IArticleService _articleService, IAuthorService _authorService, IParticipantService _participantService, 
			IBoardService _boardService, IBlockService _blockService)
		{
			_logger = logger;
			_notyf = notyf;

			articleService = _articleService;
			authorService = _authorService;
			participantService = _participantService;

			boardService = _boardService;
			blockService = _blockService;
		}

		public async Task<IActionResult> Index(string blockId, string articleId, string authorId)
		{
			int AuthorId = Convert.ToInt32(authorId);
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);

			Author find_author = await authorService.GetElement(new Author { Id = AuthorId });
			if (find_author.ParticipantId != null) ViewBag.Participant = participantService.GetElement(
				new Participant { Id = (int)find_author.ParticipantId });

			// Статьи автора
			List<Article> articles = await articleService.GetArticlesAuthor(find_author.Id);
			ViewBag.Articles = articles;

			// Статья, с которой был осуществлён переход к автору
			Article find_article = await articleService.GetElement(new Article { Id = ArticleId });
			ViewBag.Article = find_article;

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

			return View(find_author);
		}

		// Добавление нового автора
		[HttpGet]
		public async Task<IActionResult> AddNewAuthor(string blockId, string articleId)
		{
			// Передача id блока, на котором будет находиться элемент
			ViewData["blockId"] = blockId;
			ViewData["articleId"] = articleId;

			// Передача авторов, которые ещё таковыми не являются 
			List<Participant> partNoAuthor = new List<Participant>();
			List<Participant> participants = await participantService.GetFullList();
			List<Author> authors = await authorService.GetFullList();
			foreach (var part in participants) 
			{
				bool isAuthor = false;
				foreach (var author in authors) 
				{
					if (author.ParticipantId == part.Id) 
					{
						isAuthor = true;
					}
				}
				if (!isAuthor) partNoAuthor.Add(part);
			}
			ViewBag.Participants = partNoAuthor;
			return View();
		}
		[HttpPost]
		public async Task AddNewAuthor(string blockId, string articleId, 
			string name, string organization, string email, string partId)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(articleId) && 
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(organization) &&
				!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(partId))
			{
				try
				{
					// ID блока и статьи
					int BlockId = Convert.ToInt32(blockId);
					int ArticleId = Convert.ToInt32(articleId);
					int PartId = Convert.ToInt32(partId);
					int? participant = null;

					// Выбранный участник
					if (PartId != 0)
					{
						participant = PartId;
					}

					// Добавление автора и прикрепление к статье
					await authorService.Insert(new Author { AuthorFIO = name, AuthorEmail = email, 
						AuthorOrganization = organization, ParticipantId = participant });
					Author new_author = await authorService.GetElement(new Author
					{
						ParticipantId = participant,
						AuthorFIO = name,
						AuthorEmail = email,
						AuthorOrganization = organization
					});
					await articleService.GetAuthor(new_author, ArticleId);
					Response.Redirect($"/author/index?" +
						$"blockId={BlockId}" +
						$"&articleId={ArticleId}" +
						$"&authorId={new_author.Id}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/author/addnewauthor?blockId=" + blockId + "&articleId=" + articleId);
				}				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/author/addnewauthor?blockId=" + blockId + "&articleId=" + articleId);
			}
		}

		// Добавление существующего автора
		[HttpGet]
		public async Task<IActionResult> AddAuthor(string blockId, string articleId)
		{
			// Передача id
			ViewData["blockId"] = blockId;
			ViewData["articleId"] = articleId;

			int ArticleId = Convert.ToInt32(articleId);

			// Передача авторов, которые можно добавить к статье
			List<Author> all_authors = await authorService.GetFullList();
			List<Author> article_authors = await authorService.GetFilteredList(ArticleId);

			List<Author> authors_for_adds = new List<Author>();
			foreach (var auth in all_authors)
			{
				bool add = true;
				foreach (var article_auth in article_authors)
				{
					if (auth.Id == article_auth.Id) add = false;
				}
				if (add) { authors_for_adds.Add(auth); }
			}
			ViewBag.Authors = authors_for_adds;
			return View();
		}
		[HttpPost]
		public async Task AddAuthor(string blockId, string articleId, string authorId)
		{
			// Привязка мероприятия к блоку и отображение блока с мероприятиями
			int ArticleId = Convert.ToInt32(articleId);
			int BlockId = Convert.ToInt32(blockId);
			int AuthorId = Convert.ToInt32(authorId);

			Article find_article = await articleService.GetElement(new Article { Id = ArticleId });
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });
			Author find_author = await authorService.GetElement(new Author { Id = AuthorId });

			if (find_article != null && find_author != null)
			{
				try
				{
					await articleService.GetAuthor(find_author, find_article.Id);
					Response.Redirect($"/article/index?" +
						$"blockId={blockId}" +
						$"&articleId={find_article.Id}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/author/addauthor?blockId=" + blockId + "&articleId=" + articleId);
				}
			}
			else
			{
				_notyf.Error("Выберите автора");
				Response.Redirect($"/author/addauthor?blockId=" + blockId + "&articleId=" + articleId);
			}
		}

		// Редактирование автора
		[HttpGet]
		public async Task<IActionResult> UpdAuthor(string blockId, string articleId, string authorId)
		{
			// Передача id блока, на котором будет находиться элемент
			ViewData["blockId"] = blockId;
			ViewData["articleId"] = articleId;

			int AuthorId = Convert.ToInt32(authorId);
			Author find_author = await authorService.GetElement(new Author { Id = AuthorId });

			// Передача авторов, которые ещё таковыми не являются 
			List<Participant> partNoAuthor = new List<Participant>();
			List<Participant> participants = await participantService.GetFullList();
			List<Author> authors = await authorService.GetFullList();
			foreach (var part in participants)
			{
				bool isAuthor = false;
				foreach (var author in authors)
				{
					if (author.ParticipantId == part.Id)
					{
						isAuthor = true;
					}
				}
				if (!isAuthor) partNoAuthor.Add(part);
			}
			Participant? act_part = null;
			if (find_author.ParticipantId != null) 
			{ 
				act_part = await participantService.GetElement(new Participant { Id = (int)find_author.ParticipantId });
				ViewBag.ActiveParticipant = act_part;
			}
			ViewBag.Participants = partNoAuthor;

			return View(find_author);
		}
		[HttpPost]
		public async Task UpdAuthor(string id, string blockId, string articleId,
			string name, string organization, string email, string partId)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(articleId) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(organization) &&
				!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(partId))
			{
				try
				{
					// ID блока и статьи
					int AuthorId = Convert.ToInt32(id);
					int BlockId = Convert.ToInt32(blockId);
					int ArticleId = Convert.ToInt32(articleId);
					int PartId = Convert.ToInt32(partId);
					int? participant = null;

					// Выбранный участник
					if (PartId != 0)
					{
						participant = PartId;
					}

					// Добавление автора и прикрепление к статье
					await authorService.Update(new Author
					{
						Id = AuthorId,
						AuthorFIO = name,
						AuthorEmail = email,
						AuthorOrganization = organization,
						ParticipantId = participant
					});
					Author new_author = await authorService.GetElement(new Author
					{
						Id = AuthorId,
						ParticipantId = participant,
						AuthorFIO = name,
						AuthorEmail = email,
						AuthorOrganization = organization
					});
					Response.Redirect($"/author/index?" +
						$"blockId={BlockId}" +
						$"&articleId={ArticleId}" +
						$"&authorId={new_author.Id}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/author/updauthor?authorId=" + id + "blockId=" + blockId + "&articleId=" + articleId);
				}				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/author/updauthor?authorId=" + id + "blockId=" + blockId + "&articleId=" + articleId);
			}
		}

		// Удаление автора из статьи
		[HttpGet]
		public async Task DeleteAuthorArticle(string blockId, string articleId, string authorId)
		{
			if (!string.IsNullOrEmpty(articleId) && !string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(authorId))
			{
				int author_id;
				int block_id;
				int article_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_articleId = int.TryParse(articleId, out article_id);
				bool isNumeric_authorId = int.TryParse(authorId, out author_id);
				if (isNumeric_blockId && isNumeric_articleId && isNumeric_authorId)
				{
					try
					{
						Author auth = await authorService.GetElement(new Author { Id = author_id });
						Article art = await articleService.GetElement(new Article { Id = article_id });
						Block bl = await blockService.GetElement(new Block { Id = block_id });
						if (auth != null && bl != null && art != null)
						{
							await articleService.GetAuthor(auth, art.Id);
							Response.Redirect($"/article/index?" +
								$"blockId={bl.Id}" +
								$"&articleId={art.Id}");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/article/index?" +
							$"blockId={blockId}" +
							$"&articleId={articleId}");
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/article/index?" +
						$"blockId={blockId}" +
						$"&articleId={articleId}");
				}
			}
		}

		// Удаление автора
		[HttpGet]
		public async Task DeleteAuthor(string blockId, string articleId, string authorId)
		{
			if (!string.IsNullOrEmpty(articleId) && !string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(authorId))
			{
				int author_id;
				int block_id;
				int article_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_articleId = int.TryParse(articleId, out article_id);
				bool isNumeric_authorId = int.TryParse(authorId, out author_id);

				if (isNumeric_blockId && isNumeric_articleId && isNumeric_authorId)
				{
					try
					{
						Author auth = await authorService.GetElement(new Author { Id = author_id });
						Article art = await articleService.GetElement(new Article { Id = article_id });
						Block bl = await blockService.GetElement(new Block { Id = block_id });
						if (auth != null && bl != null && art != null)
						{
							await authorService.Delete(auth);
							Response.Redirect($"/article/index?" +
								$"blockId={bl.Id}" +
								$"&articleId={art.Id}");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/author/index?" +
							$"blockId={blockId}" +
							$"&articleId={articleId}" +
							$"&authorId={authorId}");
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/author/index?" +
						$"blockId={blockId}" +
						$"&articleId={articleId}" +
						$"&authorId={authorId}");
				}
			}
		}
	}
}
