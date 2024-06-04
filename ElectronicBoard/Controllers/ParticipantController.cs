using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Controllers
{
	public class ParticipantController : Controller
	{
		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly SignInManager<IdentityUser<int>> _signInManager;
		private readonly JWTSettings _options;
		private readonly ILogger<ParticipantController> _logger;
		private readonly INotyfService _notyf;

		private readonly IParticipantService participantService;
		private readonly IUserLDAPService userLDAPService;

		private readonly IStickerService stickerService;
		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IFileService fileService;

		private readonly IArticleService articleService;
		private readonly IAuthorService authorService;
		private readonly IdnMapping idn;

		public ParticipantController(UserManager<IdentityUser<int>>  user, SignInManager<IdentityUser<int>> signIn, IOptions<JWTSettings> optAccess,ILogger<ParticipantController> logger,
			INotyfService notyf, IStickerService _stickerService, IParticipantService _participantService,
			IBoardService _boardService, IBlockService _blockService, IFileService _fileService,
			IArticleService _articleService, IAuthorService _authorService, IUserLDAPService _userLDAPService)
		{
			_userManager = user;
			_signInManager = signIn;
			_options = optAccess.Value;
			_logger = logger;
			participantService = _participantService;
			boardService = _boardService;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			stickerService = _stickerService;
			fileService = _fileService;
			articleService = _articleService;
			authorService = _authorService;
			userLDAPService = _userLDAPService;
			idn = new IdnMapping();
		}
		private async Task<Participant> createTest() 
		{
			if ((await participantService.GetFullList()).Count == 0)
			{
				await Register(new Participant
				{
					ParticipantFIO = "Проверяющий",
					ParticipantPatents = "нет",
					ParticipantPublications = "нет",
					ParticipantRating = 0,
					ParticipantTasks = "нет",
					ScientificInterests = "нет",
					Picture = new byte[] { },
					Login = "Проверяющий",
					Password = "Ulstu_73"
				});

				Participant part = await participantService.GetElement(new Participant
				{
					ParticipantFIO = "Проверяющий"
				});
				return part;
			}
			else 
			{
				return await participantService.GetElement(new Participant
				{
					ParticipantFIO = "Проверяющий"
				});
			}
		}

		// Регистрация
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

				await _userManager.AddClaimsAsync(user, claims);

				var new_user = await _userManager.FindByNameAsync(part.Login);
				if (new_user != null) 
				{ 
					part.IdentityId = new_user.Id;
					await participantService.CreateTestParticipant(part); 
				}
			}
			else 
			{
				throw new Exception("Ошибка регистрации участника");
			}
		}

		// Получение токена аутентификации
		[HttpGet]
		public string GetToken(IdentityUser<int> user, IEnumerable<Claim> pr)
		{
			var claims = pr.ToList();
			claims.Add(new Claim("Login", user.UserName));

			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

			var jwt = new JwtSecurityToken(
				notBefore: DateTime.UtcNow,
				issuer: _options.Issuer,
				audience: _options.Audience,
				claims: claims,
				expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)),
				signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
				);
			return new JwtSecurityTokenHandler().WriteToken(jwt);
		}

		// Вход
		[HttpGet]
		public async Task<IActionResult> Enter()
		{
			createTest().Wait();
			return View();
		}
		[HttpPost]
		public async Task Enter(string login, string password)
		{
			/*
			var user_app = await _userManager.FindByNameAsync(login);

			IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user_app);
			var token = GetToken(user_app, claims);

			HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", token,
			new CookieOptions
			{
				MaxAge = TimeSpan.FromMinutes(60)
			});

			Response.Redirect($"/board/index?" +
				$"BoardName={idn.GetAscii("Общая доска")}");
			*/
			
			if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
			{
				string hashedPassword = "{CRYPT}" + BCrypt.Net.BCrypt.HashPassword(password, 10);

				// 1. Если учётка есть и пароль введён верно => 
				// обращение к пользователям приложения
				var userLDAP = await userLDAPService.GetElement(new UserLDAP { UserLogin = login });
				if (userLDAP != null && userLDAP.UserPassword.Contains(hashedPassword))
				{
					var user_app = await _userManager.FindByNameAsync(login);
					// 1.1. Если пользователь по логину найден
					if (user_app != null)
					{
						var result_app = await _signInManager.PasswordSignInAsync(user_app, hashedPassword, false, false);

						// если пароль записан верно => вход
						if (result_app.Succeeded)
						{
							IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user_app);
							var token = GetToken(user_app, claims);

							HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", token,
							new CookieOptions
							{
								MaxAge = TimeSpan.FromMinutes(60)
							});

							Response.Redirect($"/board/index?" +
								$"BoardName={idn.GetAscii("Общая доска")}");
						}
						else
						{
							// если пароль записан пользователю неверно => изменение пароля и вход

							Participant login_participant = await participantService.GetElement(new Participant { IdentityId = user_app.Id });
							await participantService.Update(new Participant { Id = login_participant.Id, Password = hashedPassword });

							var result_app2 = await _signInManager.PasswordSignInAsync(user_app, hashedPassword, false, false);

							if (result_app2.Succeeded)
							{
								IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user_app);
								var token = GetToken(user_app, claims);

								HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", token,
								new CookieOptions
								{
									MaxAge = TimeSpan.FromMinutes(60)
								});

								Response.Redirect($"/board/index?" +
									$"BoardName={idn.GetAscii("Общая доска")}");
							}
							else
							{
								_notyf.Error("Введите логин и пароль");
								Response.Redirect($"/participant/enter");
							}
						}
					}
					else 
					{
						// 1.2. Если пользователь по логину не найден
						// регистрация пользователя в приложении и вход
						await Register(new Participant
						{
							ParticipantFIO = userLDAP.UserFIO,
							ParticipantPatents = "нет",
							ParticipantPublications = "нет",
							ParticipantRating = 0,
							ParticipantTasks = "нет",
							ScientificInterests = "нет",
							Picture = new byte[] { },
							Login = userLDAP.UserLogin,
							Password = userLDAP.UserPassword
						});

						var user_reg = await _userManager.FindByNameAsync(login);
						var result_reg = await _signInManager.PasswordSignInAsync(user_reg, hashedPassword, false, false);

						IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user_reg);
						var token = GetToken(user_reg, claims);

						HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", token,
						new CookieOptions
						{
							MaxAge = TimeSpan.FromMinutes(60)
						});

						Response.Redirect($"/board/index?" +
							$"BoardName={idn.GetAscii("Общая доска")}");
					}
				}
				else 
				{
					// 2. Если такой учётки нет
					_notyf.Error("Введите логин и пароль");
					Response.Redirect($"/participant/enter");
				}

				var user = await _userManager.FindByNameAsync(login);
				var result = await _signInManager.PasswordSignInAsync(user, hashedPassword, false, false);

				if (result.Succeeded)
				{
					IEnumerable<Claim> claims = await _userManager.GetClaimsAsync(user);
					var token = GetToken(user, claims);

					HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", token,
					new CookieOptions
					{
						MaxAge = TimeSpan.FromMinutes(60)
					});

					Response.Redirect($"/board/index?" +
						$"BoardName={idn.GetAscii("Общая доска")}");
				}
				else 
				{
					_notyf.Error("Введите логин и пароль");
					Response.Redirect($"/participant/enter");
				}
			}
			else
			{
				_notyf.Error("Введите логин и пароль");
				Response.Redirect($"/participant/enter");
			}
		}

		// Выход 
		[HttpGet]
		public async Task Logout()
		{
			// удаляем аутентификационные куки
			await _signInManager.SignOutAsync();
			Response.Redirect($"/participant/enter");
		}

		// Редактировать профиль
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> UpdProfile(Participant participant)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			Participant find_element = await participantService.GetElement(new Participant { Id = participant.Id });
			if (find_element != null) 
			{
				return View(find_element);
			}
			else
			{
				_notyf.Error("Ошибка");
				return Redirect("javascript: history.go(-1)");
			}
		}
		[Authorize]
		[HttpPost]
		public async Task UpdProfile(string id,
			string name, string patents, string publics, string tasks, string interests, IFormFile pict, string delpic)
		{
			if (!string.IsNullOrEmpty(id) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(patents) &&
				!string.IsNullOrEmpty(publics) && !string.IsNullOrEmpty(tasks) && !string.IsNullOrEmpty(interests))
			{
				Participant find_element = await participantService.GetElement(new Participant { Id = Convert.ToInt32(id) });
				if (find_element != null)
				{
					try
					{
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
							picture = (await participantService.GetElement(new Participant { Id = find_element.Id })).Picture;
						}

						// Редактирование и отображение профиля
						await participantService.Update(new Participant
						{
							Id = find_element.Id,
							Picture = picture,
							ParticipantFIO = name,
							ParticipantPatents = patents,
							ParticipantPublications = publics,
							ParticipantRating = 0,
							ParticipantTasks = tasks,
							ScientificInterests = interests
						});
						Response.Redirect($"/participant/profile?partId={idn.GetAscii(find_element.Id.ToString())}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/participant/updprofile?Id={idn.GetAscii(id.ToString())}");
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
				Response.Redirect($"/participant/updprofile?Id={idn.GetAscii(id.ToString())}");
			}
		}

		// Обновить рейтинг
		[Authorize]
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
				Response.Redirect($"/board/index?BoardName={idn.GetAscii("Общая доска")}");
			}
			Response.Redirect($"/participant/profile?partId={idn.GetAscii(partId.ToString())}");
		}

		[Authorize]
		public async Task<IActionResult> Profile(string partId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

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
				return View(find_part);
			}
			else
			{
				_notyf.Error("Участник не найден");
				return Redirect($"/board/index?BoardName={idn.GetAscii("Общая доска")}");
			}
		}
		[Authorize]
		public async Task<IActionResult> Index(string partId, string blockId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

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
				Response.Redirect($"/block/index?Id={idn.GetAscii(blockId)}");
			}
			return View(find_part);
		}

		// Прикрепление участника к доске
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> AddPartBlock(string blockId)
		{
			IdentityUser<int> UserId = await _userManager.GetUserAsync(HttpContext.User);
			Participant activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
			ViewBag.ActivePart = activeUser;

			List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
			ViewBag.ActiveBoards = activeBoards;

			int BlockId = Convert.ToInt32(blockId);
			Block find_block = await blockService.GetElement(new Block { Id = BlockId });
			ViewBag.Block = find_block;

			if (find_block != null)
			{
				List<Participant> all_part = await participantService.GetFullList();
				List<Participant> block_part = await participantService.GetFilteredList(find_block.Id);

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
				Response.Redirect($"/block/index?Id={idn.GetAscii(blockId.ToString())}");
			}
			return View();
		}
		[Authorize]
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
					Response.Redirect($"/block/index?Id={idn.GetAscii(find_block.Id.ToString())}&BoardId={idn.GetAscii(find_block.BoardId.ToString())}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/participant/addpartblock?blockId=" + idn.GetAscii(blockId));
				}
			}
			else
			{
				_notyf.Error("Выберите участника");
				Response.Redirect($"/participant/addpartblock?blockId=" + idn.GetAscii(blockId));
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