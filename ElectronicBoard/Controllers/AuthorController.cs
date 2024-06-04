using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ElectronicBoard.Controllers
{
	[Authorize]
	public class AuthorController : Controller
	{
		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly ILogger<AuthorController> _logger;
		private readonly INotyfService _notyf;
		private readonly IdnMapping idn;

		private readonly IArticleService articleService;
		private readonly IAuthorService authorService;
		private readonly IParticipantService participantService;

		private readonly IBoardService boardService;
		private readonly IBlockService blockService;

		public AuthorController(ILogger<AuthorController> logger, INotyfService notyf, 
			IArticleService _articleService, IAuthorService _authorService, IParticipantService _participantService, 
			IBoardService _boardService, IBlockService _blockService, UserManager<IdentityUser<int>> userManager)
		{
			_logger = logger;
			_notyf = notyf;
			_userManager = userManager;
			idn = new IdnMapping();

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

			IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Author find_author = await authorService.GetElement(new Author { Id = AuthorId });
			Article? find_article = await articleService.GetElement(new Article { Id = ArticleId });
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });

			if (find_author != null && find_article != null && find_block != null)
			{
				if (find_author.ParticipantId != null)
					ViewBag.Participant = participantService.GetElement(
					new Participant { Id = (int)find_author.ParticipantId });

				// Статьи автора
				List<Article> articles = await articleService.GetArticlesAuthor(find_author.Id);
				ViewBag.Articles = articles;

				// Статья, с которой был осуществлён переход к автору
				ViewBag.Article = find_article;

				// Блок, на котором находится элемент
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
			else 
			{
				_notyf.Error("Автор не найден");
				return Redirect("javascript: history.go(-1)");
			}
		}

		// Добавление нового автора
		[HttpGet]
		public async Task<IActionResult> AddNewAuthor(string blockId, string articleId)
		{
			IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Article? find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
			Block find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

			if (find_article != null && find_block != null) 
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
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task AddNewAuthor(string blockId, string articleId, 
			string name, string organization, string email, string partId)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(articleId) && 
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(organization) &&
				!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(partId))
			{
				Article? find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
				Block find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

				if (find_article != null && find_block != null)
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
						await authorService.Insert(new Author
						{
							AuthorFIO = name,
							AuthorEmail = email,
							AuthorOrganization = organization,
							ParticipantId = participant
						});
						Author new_author = await authorService.GetElement(new Author
						{
							ParticipantId = participant,
							AuthorFIO = name,
							AuthorEmail = email,
							AuthorOrganization = organization
						});
						await articleService.GetAuthor(new_author, ArticleId);
						Response.Redirect($"/author/index?" +
							$"blockId={idn.GetAscii(BlockId.ToString())}" +
							$"&articleId={idn.GetAscii(ArticleId.ToString())}" +
							$"&authorId={idn.GetAscii(new_author.Id.ToString())}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/author/addnewauthor?blockId=" + idn.GetAscii(blockId) + "&articleId=" + idn.GetAscii(articleId));
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
				Response.Redirect($"/author/addnewauthor?blockId=" + idn.GetAscii(blockId) + "&articleId=" + idn.GetAscii(articleId));
			}
		}

		// Добавление существующего автора
		[HttpGet]
		public async Task<IActionResult> AddAuthor(string blockId, string articleId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Article? find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
			Block find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

			if (find_article != null && find_block != null)
			{
				// Передача id
				ViewData["blockId"] = find_block.Id;
				ViewData["articleId"] = find_article.Id;

				// Передача авторов, которые можно добавить к статье
				List<Author> all_authors = await authorService.GetFullList();
				List<Author> article_authors = await authorService.GetFilteredList(find_article.Id);

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
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}

		}
		[HttpPost]
		public async Task AddAuthor(string blockId, string articleId, string authorId)
		{
			// Привязка мероприятия к блоку и отображение блока с мероприятиями
			Article find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
			Block find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
			Author find_author = await authorService.GetElement(new Author { Id = Convert.ToInt32(authorId) });

			if (find_article != null && find_author != null && find_block != null)
			{
				try
				{
					await articleService.GetAuthor(find_author, find_article.Id);
					Response.Redirect($"/article/index?" +
						$"blockId={idn.GetAscii(blockId)}" +
						$"&articleId={idn.GetAscii(find_article.Id.ToString())}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/author/addauthor?blockId=" + find_block.Id + "&articleId=" + find_article.Id);
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
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Article find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
			Block find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
			Author find_author = await authorService.GetElement(new Author { Id = Convert.ToInt32(authorId) });

			if (find_article != null && find_block != null && find_author != null) 
			{
				// Передача id блока, на котором будет находиться элемент
				ViewData["blockId"] = find_block.Id;
				ViewData["articleId"] = find_article.Id;

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
				if (find_author.ParticipantId != null)
				{
					ViewBag.ActiveParticipant = await participantService.GetElement(new Participant { Id = (int)find_author.ParticipantId });
				}
				ViewBag.Participants = partNoAuthor;
				return View(find_author);
			}
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task UpdAuthor(string id, string blockId, string articleId,
			string name, string organization, string email, string partId)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(articleId) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(organization) &&
				!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(partId))
			{
				Author find_author = await authorService.GetElement(new Author { Id = Convert.ToInt32(id) });
				Article find_article = await articleService.GetElement(new Article { Id = Convert.ToInt32(articleId) });
				Block find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

				if (find_author != null && find_article != null && find_block != null)
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
							$"blockId={idn.GetAscii(BlockId.ToString())}" +
							$"&articleId={idn.GetAscii(ArticleId.ToString())}" +
							$"&authorId={idn.GetAscii(new_author.Id.ToString())}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/author/updauthor?authorId=" + idn.GetAscii(id) + "blockId=" + idn.GetAscii(blockId) + "&articleId=" + idn.GetAscii(articleId));
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
				Response.Redirect($"/author/updauthor?authorId=" + idn.GetAscii(id) + "blockId=" + idn.GetAscii(blockId) + "&articleId=" + idn.GetAscii(articleId));
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
								$"blockId={idn.GetAscii(bl.Id.ToString())}" +
								$"&articleId={idn.GetAscii(art.Id.ToString())}");
						}
						else
						{
							_notyf.Error("Ошибка");
							Response.Redirect("javascript: history.go(-1)");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/article/index?" +
							$"blockId={idn.GetAscii(blockId)}" +
							$"&articleId={idn.GetAscii(articleId)}");
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/article/index?" +
						$"blockId={idn.GetAscii(blockId)}" +
						$"&articleId={idn.GetAscii(articleId)}");
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
								$"blockId={idn.GetAscii(bl.Id.ToString())}" +
								$"&articleId={idn.GetAscii(art.Id.ToString())}");
						}
						else
						{
							_notyf.Error("Ошибка");
							Response.Redirect("javascript: history.go(-1)");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/author/index?" +
							$"blockId={idn.GetAscii(blockId)}" +
							$"&articleId={idn.GetAscii(articleId)}" +
							$"&authorId={idn.GetAscii(authorId)}");
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/author/index?" +
						$"blockId={idn.GetAscii(blockId)}" +
						$"&articleId={idn.GetAscii(articleId)}" +
						$"&authorId={idn.GetAscii(authorId)}");
				}
			}
		}
	}
}
