using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicBoard.Controllers
{
	/// <summary>
	/// Контроллер, обрабатывающий запросы касающиеся простых элементов
	/// </summary>
	[Authorize]
	public class ElementController : Controller
	{
		private readonly ILogger<ElementController> _logger;
		private readonly IdnMapping idn;

		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly IParticipantService participantService;

		private readonly ISimpleElementService elementService;
		private readonly IStickerService stickerService;
		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IFileService fileService;
		private readonly INotyfService _notyf;

		public ElementController(ILogger<ElementController> logger, INotyfService notyf, 
			ISimpleElementService _elementService, IStickerService _stickerService,
			IBoardService _boardService, IBlockService _blockService, IFileService _fileService,
			UserManager<IdentityUser<int>> userManager, IParticipantService _participantService)
		{
			_logger = logger;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			elementService = _elementService;
			stickerService = _stickerService;
			fileService = _fileService;
			idn = new IdnMapping();
			participantService = _participantService;
			_userManager = userManager;
		}

		/// <summary>
		/// Метод для отображения страницы с информацией о простом элементе
		/// </summary>
		/// <param name="simpleElement"></param>
		/// <returns></returns>
		public async Task<IActionResult> Index(SimpleElement simpleElement)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				SimpleElement? find_element = await elementService.GetElement(new SimpleElement { Id = simpleElement.Id, SimpleElementName = idn.GetUnicode(simpleElement.SimpleElementName) });

				if (find_element != null)
				{
					// Конвертация изображения
					if (find_element.Picture.Length > 0)
					{
						ViewBag.Picture = "data:image/jpg;base64," + Convert.ToBase64String(find_element.Picture);
					}

					// Стикеры
					List<Sticker> stickers = await stickerService.GetFilteredList("element", find_element.Id);
					ViewBag.Stickers = stickers;

					// Файлы
					List<File> files = await fileService.GetFilteredList("element", find_element.Id);
					ViewBag.Files = files;

					// Блок, на котором находится элемент
					Block? find_block = await blockService.GetElement(new Block { Id = find_element.BlockId });
					ViewBag.Block = find_block;

					// Доска, на которой находится блок
					Board? board = await boardService.GetElement(new Board { Id = find_block.BoardId });
					List<Block> added_blocks = new List<Block>();
					foreach (var b in await blockService.GetFilteredList(new Block { BoardId = board.Id })) { added_blocks.Add(b); }
					ViewBag.Board = new Board
					{
						Id = board.Id,
						BoardName = board.BoardName,
						BoardThematics = board.BoardThematics,
						Blocks = added_blocks
					};
					return View(find_element);
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

		/// <summary>
		/// Метод для отображения страницы добавления простого элемента
		/// </summary>
		/// <param name="blockId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddElement(string blockId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Block? block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				if (block != null)
				{
					// Передача id блока, на котором будет находиться элемент
					ViewData["blockId"] = block.Id;
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
		public async Task AddElement(string blockId, string name, string text, IFormFile pict)
		{
			if (!string.IsNullOrEmpty(blockId) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
			{
				try
				{
					Block? block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
					if (block != null)
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

						// ID блока
						int BlockId = Convert.ToInt32(blockId);

						// Добавление и отображение элемента блока
						await elementService.Insert(new SimpleElement 
						{ 
							SimpleElementName = name, 
							SimpleElementText = text, 
							BlockId = BlockId, 
							Picture = picture
						});
						SimpleElement? new_element = await elementService.GetElement(new SimpleElement
						{
							SimpleElementName = name,
							SimpleElementText = text,
							BlockId = BlockId
						});

						Response.Redirect($"/element/index?" +
							$"Id={idn.GetAscii(new_element.Id.ToString())}" +
							$"&SimpleElementName={idn.GetAscii(new_element.SimpleElementName)}");
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
					Response.Redirect($"/element/addelement?blockId=" + idn.GetAscii(blockId));
				}				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/element/addelement?blockId=" + idn.GetAscii(blockId));
			}
		}

		/// <summary>
		/// Метод для отображения страницы редактирования простого элемента
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> UpdElement(SimpleElement element)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				SimpleElement? find_element = await elementService.GetElement(new SimpleElement
				{
					Id = element.Id,
					SimpleElementName = idn.GetUnicode(element.SimpleElementName),
					SimpleElementText = idn.GetUnicode(element.SimpleElementText),
					BlockId = element.BlockId
				});
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
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task UpdElement(string id, string blockId, 
			string name, string text, IFormFile pict, string delpic) 
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(blockId) &&
				!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
			{
				try
				{
					SimpleElement? find_element = await elementService.GetElement(new SimpleElement { Id = Convert.ToInt32(id)  });
					Block? find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

					if (find_block != null && find_element != null)
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
							var simp_el = await elementService.GetElement(new SimpleElement { Id = find_element.Id });
							if (simp_el != null) picture = simp_el.Picture;
						}

						// Редактирование и отображение элемента
						await elementService.Update(new SimpleElement
						{
							Id = find_element.Id,
							SimpleElementName = name,
							SimpleElementText = text,
							BlockId = find_block.Id,
							Picture = picture
						});
						Response.Redirect($"/element/index?" +
							$"Id={idn.GetAscii(find_element.Id.ToString())}" +
							$"BlockId={idn.GetAscii(find_block.Id.ToString())}" +
							$"&SimpleElementName={idn.GetAscii(name)}" +
							$"&SimpleElementText={idn.GetAscii(text)}");
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
					Response.Redirect($"/element/updelement?" +
						$"Id={idn.GetAscii(id)}" +
						$"BlockId={idn.GetAscii(blockId)}" +
						$"&SimpleElementName={idn.GetAscii(name)}" +
						$"&SimpleElementText={idn.GetAscii(text)}");
				}				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/element/updelement?" +
					$"Id={idn.GetAscii(id)}" +
					$"BlockId={idn.GetAscii(blockId)}" +
					$"&SimpleElementName={idn.GetAscii(name)}" +
					$"&SimpleElementText={idn.GetAscii(text)}");
			}
		}

		/// <summary>
		/// Метод для удаления простого элемента
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="elementId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task DeleteElement(string blockId, string elementId) 
		{
			if (!string.IsNullOrEmpty(elementId) && !string.IsNullOrEmpty(blockId))
			{
				SimpleElement? find_element = await elementService.GetElement(new SimpleElement { Id = Convert.ToInt32(elementId) });
				Block? find_block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });

				if (find_block != null && find_element != null)
				{
					try
					{
						await elementService.Delete(new SimpleElement { Id = find_element.Id });
						Response.Redirect($"/block/index?Id=" + idn.GetAscii(find_block.Id.ToString()));
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/element/index?Id=" + idn.GetAscii(find_element.Id.ToString()));
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/element/index?Id=" + idn.GetAscii(find_element.Id.ToString()));
				}
			}
			else
			{
				_notyf.Error("Ошибка");
				Response.Redirect("javascript: history.go(-1)");
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