using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicBoard.Controllers
{
	public class ElementController : Controller
	{
		private readonly ILogger<ElementController> _logger;
		private readonly ISimpleElementService elementService;
		private readonly IStickerService stickerService;
		private readonly IBoardService boardService;
		private readonly IBlockService blockService;
		private readonly IFileService fileService;
		private readonly INotyfService _notyf;

		public ElementController(ILogger<ElementController> logger, INotyfService notyf, 
			ISimpleElementService _elementService, IStickerService _stickerService,
			IBoardService _boardService, IBlockService _blockService, IFileService _fileService)
		{
			_logger = logger;
			_notyf = notyf;
			boardService = _boardService;
			blockService = _blockService;
			elementService = _elementService;
			stickerService = _stickerService;
			fileService = _fileService;
		}

		// Отображение страницы с информацией об элементе
		public async Task<IActionResult> Index(SimpleElement simpleElement)
		{
			SimpleElement find_element = await elementService.GetElement(new SimpleElement { Id = simpleElement.Id, SimpleElementName = simpleElement.SimpleElementName });

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
			Block find_block = await blockService.GetElement(new Block { Id = find_element.BlockId });
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
			return View(find_element);
		}

		// Добавление элемента
		[HttpGet]
		public IActionResult AddElement(string blockId)
		{
			// Передача id блока, на котором будет находиться элемент
			ViewData["blockId"] = blockId;
			return View();
		}
		[HttpPost]
		public async Task AddElement(string blockId, string name, string text, IFormFile pict)
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
					SimpleElement new_element = await elementService.GetElement(new SimpleElement
					{
						SimpleElementName = name,
						SimpleElementText = text,
						BlockId = BlockId
					});

					Response.Redirect($"/element/index?" +
						$"Id={new_element.Id}" +
						$"&SimpleElementName={new_element.SimpleElementName}" +
						$"&SimpleElementText={new_element.SimpleElementText}" +
						$"&BlockId={new_element.BlockId}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/element/addelement?blockId=" + blockId);
				}				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/element/addelement?blockId=" + blockId);
			}
		}

		// Редактирование элемента
		[HttpGet]
		public async Task<IActionResult> UpdElement(SimpleElement element)
		{
			SimpleElement find_element = await elementService.GetElement(new SimpleElement
			{
				Id = element.Id,
				SimpleElementName = element.SimpleElementName,
				SimpleElementText = element.SimpleElementText,
				BlockId = element.BlockId
			});
			return View(find_element);
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
						picture = (await elementService.GetElement(new SimpleElement { Id = ElementId })).Picture;
					}

					// Редактирование и отображение элемента
					await elementService.Update(new SimpleElement 
					{ 
						Id = ElementId, 
						SimpleElementName = name, 
						SimpleElementText = text, 
						BlockId = BlockId, 
						Picture = picture 
					});
					Response.Redirect($"/element/index?" +
						$"Id={ElementId}" +
						$"BlockId={BlockId}" +
						$"&SimpleElementName={name}" +
						$"&SimpleElementText={text}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/element/updelement?" +
						$"Id={id}" +
						$"BlockId={blockId}" +
						$"&SimpleElementName={name}" +
						$"&SimpleElementText={text}");
				}				
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/element/updelement?" +
					$"Id={id}" +
					$"BlockId={blockId}" +
					$"&SimpleElementName={name}" +
					$"&SimpleElementText={text}");
			}
		}

		// Удаление элемента
		[HttpGet]
		public void DeleteElement(string blockId, string elementId) 
		{
			if (!string.IsNullOrEmpty(elementId) && !string.IsNullOrEmpty(blockId))
			{
				int block_id;
				int element_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_elementId = int.TryParse(elementId, out element_id);
				if (isNumeric_blockId && isNumeric_elementId)
				{
					try
					{
						elementService.Delete(new SimpleElement { Id = element_id });
						Response.Redirect($"/block/index?Id=" + block_id);
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/element/index?Id=" + element_id);
					}
				}
				else
				{
					_notyf.Error("Элемент не найден");
					Response.Redirect($"/element/index?Id=" + element_id);
				}
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