using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElectronicBoard.Controllers
{
	public class BlockController : Controller
	{
		private readonly ILogger<BoardController> _logger;
		private readonly INotyfService _notyf;

		private readonly IBoardService boardService;
		private readonly IBlockService blockService;

		private readonly IParticipantService partService;
		private readonly IEventService eventService;
		private readonly IGrantService grantService;
		private readonly IProjectService projectService;
		private readonly ISimpleElementService simpleElementService;

		public BlockController(ILogger<BoardController> logger, INotyfService notyf,
			 IBoardService _boardService, IBlockService _blockService, 
			 IParticipantService _partService, IEventService _eventService, 
			 IGrantService _grantService, IProjectService _projectService, ISimpleElementService _simpleElementService)
		{
			_logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			partService = _partService;
			boardService = _boardService;
			eventService = _eventService;
			grantService = _grantService;
			projectService = _projectService;
			simpleElementService = _simpleElementService;
		}

		// Отображение блока с участниками
		public async Task<IActionResult> Index(Block block)
		{
			Block find_block = await blockService.GetElement(new Block { Id = block.Id, BlockName = block.BlockName, BoardId = block.BoardId, 
				VisibilityOpening = block.VisibilityOpening });

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

			// Получение элементов блока
			if (find_block != null && board != null)
			{
				// Участников
				if (find_block.BlockName.Contains("Участники"))
				{
					List<Participant> parts = await partService.GetFilteredList(null, find_block.Id);
					List<Participant> added_parts = new List<Participant>();
					foreach (var part in parts) { added_parts.Add(part); }
					ViewBag.Participants = added_parts;
				}
				// Мероприятия
				else if (find_block.BlockName.Contains("Мероприятия"))
				{
					List<Event> events = await eventService.GetFilteredList(null, find_block.Id);
					List<Event> added_events = new List<Event>();
					foreach (var ev in events) 
					{
						added_events.Add(ev); 
					}
					ViewBag.DateNow = DateTime.Now;
					ViewBag.Events = added_events;
				}
				// Гранты
				else if (find_block.BlockName.Contains("Гранты"))
				{
					List<Grant> grants = await grantService.GetFilteredList(find_block.Id);
					List<Grant> added_grants = new List<Grant>();
					foreach (var gr in grants) { added_grants.Add(gr); }
					ViewBag.Grants = added_grants;
				}
				// Проекты
				else if (find_block.BlockName.Contains("Проекты"))
				{
					List<Project> projects = await projectService.GetFilteredList(find_block.Id);
					List<Project> added_projects = new List<Project>();
					foreach (var pr in projects) { added_projects.Add(pr); }
					ViewBag.Projects = added_projects;
				}
				// Простые элементы
				else
				{
					List<SimpleElement> elements = await simpleElementService.GetFilteredList(find_block.Id);
					List<SimpleElement> added_elements = new List<SimpleElement>();
					foreach (var elem in elements) { added_elements.Add(elem); }
					ViewBag.Elements = added_elements;
				}
				return View(new Block
				{
					Id = find_block.Id,
					BlockName = find_block.BlockName,
					BoardId = find_block.BoardId,
					VisibilityOpening = find_block.VisibilityOpening
				});
			}
			else return View();
		}
		// Добавление доски
		[HttpGet]
		public IActionResult AddBlock(string boardId)
		{
			// Передача id доски, на которой будет находиться блок
			ViewData["boardId"] = boardId;
			return View();
		}
		[HttpPost]
		public async Task AddBlock(string boardId, string name, string copy)
		{
			if (!string.IsNullOrEmpty(boardId) && !string.IsNullOrEmpty(name))
			{
				try
				{
					int BoardId = Convert.ToInt32(boardId);

					// Видимость блока на доске
					bool visible = true;
					switch (copy) 
					{
						case "on":
							visible = true;
							break;
						case null:
							visible = false;
							break;
					}

					// Добавление и отображение блока доски
					await blockService.Insert(new Block { BoardId = BoardId, BlockName = name, VisibilityOpening = visible }) ;
					Block new_block = await blockService.GetElement(new Block
					{
						BoardId = BoardId,
						BlockName = name,
						VisibilityOpening = visible
					});

					Response.Redirect($"/block/index?" +
						$"Id={new_block.Id}" +
						$"&BoardId={new_block.BoardId}" +
						$"&BlockName={new_block.BlockName}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/block/addblock?boardId=" + boardId);
				}
			}
			else 
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/block/addblock?boardId=" + boardId);
			}
		}
		//Удаление блока
		[HttpGet]
		public async Task DeleteBlock(string blockId, string boardId) 
		{
			if (!string.IsNullOrEmpty(blockId)) 
			{
				int block_id;
				int board_id;
				bool isNumeric_blockId = int.TryParse(blockId, out block_id);
				bool isNumeric_boardId = int.TryParse(boardId, out board_id);
				if (isNumeric_blockId && isNumeric_boardId)
				{
					try
					{
						await blockService.Delete(new Block { Id = block_id });
						Response.Redirect($"/board/index?Id=" + board_id);
					}
					catch (Exception ex) 
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/block/index?Id=" + block_id);
					}
				}
				else 
				{
					_notyf.Error("Блок не найден");
					Response.Redirect($"/block/index?Id=" + block_id);
				}
			}
		}

		// Редактирование блока
		[HttpGet]
		public async Task<IActionResult> UpdBlock(Block block)
		{
			Block find_block = await blockService.GetElement(new Block { Id = block.Id, BoardId = block.BoardId, BlockName = block.BlockName, VisibilityOpening = block.VisibilityOpening });
			return View(find_block);
		}
		[HttpPost]
		public async Task UpdBlock(string id, string boardId, string name, string copy)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(boardId) && !string.IsNullOrEmpty(name))
			{
				try
				{
					int BlockId = Convert.ToInt32(id);
					int BoardId = Convert.ToInt32(boardId);

					// Видимость
					bool visible = true;
					switch (copy)
					{
						case "on":
							visible = true;
							break;
						case null:
							visible = false;
							break;
					}

					// Редактирование и отображение блока
					await blockService.Update(new Block { Id = BlockId, BoardId = BoardId, BlockName = name, VisibilityOpening = visible });
					Response.Redirect($"/block/index?" +
						$"Id={BlockId}" +
						$"BoardId={BoardId}" +
						$"&BlockName={name}" +
						$"&VisibilityOpening={visible}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/block/updblock?" +
						$"Id={id}" +
						$"BoardId={boardId}" +
						$"&BlockName={name}" +
						$"&VisibilityOpening={copy}");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/block/updblock?" +
					$"Id={id}" +
					$"BoardId={boardId}" +
					$"&BlockName={name}" +
					$"&VisibilityOpening={copy}");
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}