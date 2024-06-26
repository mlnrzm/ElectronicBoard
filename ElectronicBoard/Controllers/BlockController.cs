﻿using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;

namespace ElectronicBoard.Controllers
{
	/// <summary>
	/// Контроллер, обрабатывающий запросы касающиеся блоков
	/// </summary>
	[Authorize]
	public class BlockController : Controller
	{
		private readonly ILogger<BoardController> _logger;
		private readonly INotyfService _notyf;
		private readonly IdnMapping idn;

		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly IParticipantService participantService;

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
			 IGrantService _grantService, IProjectService _projectService, ISimpleElementService _simpleElementService,
			 UserManager<IdentityUser<int>> userManager, IParticipantService _participantService)
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
			idn = new IdnMapping();
			_userManager = userManager;
			participantService = _participantService;
		}

		/// <summary>
		/// Метод для отображения страницы с информацией о блоке
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		public async Task<IActionResult> Index(Block block)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Block? find_block = await blockService.GetElement(new Block { Id = block.Id });

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

				// Получение элементов блока
				if (find_block != null && board != null)
				{
					// Участников
					if (find_block.BlockName.Contains("Участники"))
					{
						List<Participant> parts = await partService.GetFilteredList(find_block.Id);
						List<Participant> added_parts = new List<Participant>();
						foreach (var part in parts) { added_parts.Add(part); }
						ViewBag.Participants = added_parts;
					}
					// Мероприятия
					else if (find_block.BlockName.Contains("Мероприятия"))
					{
						List<Event> events = await eventService.GetFilteredList(find_block.Id);
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
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}

		/// <summary>
		/// Метод для отображения страницы добавления блока
		/// </summary>
		/// <param name="boardId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddBlock(string boardId)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				var board = await boardService.GetElement(new Board { Id = Convert.ToInt32(boardId) });

				if (board != null)
				{
					// Передача id доски, на которой будет находиться блок
					ViewData["boardId"] = boardId;
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
		public async Task AddBlock(string boardId, string name, string copy)
		{
			if (!string.IsNullOrEmpty(boardId) && !string.IsNullOrEmpty(name))
			{
				try
				{
					var board = await boardService.GetElement(new Board { Id = Convert.ToInt32(boardId) });
					if (board != null)
					{
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
						await blockService.Insert(new Block { BoardId = board.Id, BlockName = name, VisibilityOpening = visible });
						Block? new_block = await blockService.GetElement(new Block
						{
							BoardId = board.Id,
							BlockName = name,
							VisibilityOpening = visible
						});

						Response.Redirect($"/block/index?" +
							$"Id={idn.GetAscii(new_block.Id.ToString())}" +
							$"&BoardId={idn.GetAscii(new_block.BoardId.ToString())}" +
							$"&BlockName={idn.GetAscii(new_block.BlockName)}");
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
					Response.Redirect($"/block/addblock?boardId=" + idn.GetAscii(boardId));
				}
			}
			else 
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/block/addblock?boardId=" + idn.GetAscii(boardId));
			}
		}

		/// <summary>
		/// Метод для удаления блока
		/// </summary>
		/// <param name="blockId"></param>
		/// <param name="boardId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task DeleteBlock(string blockId, string boardId) 
		{
			if (!string.IsNullOrEmpty(blockId)) 
			{
				Block? block = await blockService.GetElement(new Block { Id = Convert.ToInt32(blockId) });
				Board? board = await boardService.GetElement(new Board { Id = Convert.ToInt32(boardId) });

				if (block != null && board != null)
				{
					try
					{
						await blockService.Delete(new Block { Id = block.Id });
						Response.Redirect($"/board/index?" +
							$"Id={idn.GetAscii(board.Id.ToString())}" +
							$"&BoardName={idn.GetAscii(board.BoardName)}" +
							$"&BoardThematics={idn.GetAscii(board.BoardThematics)}");
					}
					catch (Exception ex) 
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/block/index?Id=" + idn.GetAscii(block.Id.ToString()));
					}
				}
				else 
				{
					_notyf.Error("Блок не найден");
					Response.Redirect($"/block/index?Id=" + idn.GetAscii(block.Id.ToString()));
				}
			}
			else
			{
				_notyf.Error("Ошибка");
				Response.Redirect("javascript: history.go(-1)");
			}
		}

		/// <summary>
		/// Метод для отображения страницы редактирования блока
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> UpdBlock(Block block)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Block? find_block = await blockService.GetElement(new Block { Id = block.Id, BoardId = block.BoardId, BlockName = idn.GetUnicode(block.BlockName), VisibilityOpening = block.VisibilityOpening });

				if (find_block != null)
				{
					return View(find_block);
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
		public async Task UpdBlock(string id, string boardId, string name, string copy)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(boardId) && !string.IsNullOrEmpty(name))
			{
				try
				{
					Block? block = await blockService.GetElement(new Block { Id = Convert.ToInt32(id) });
					Board? board = await boardService.GetElement(new Board { Id = Convert.ToInt32(boardId) });

					if (block != null && board != null)
					{
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
						await blockService.Update(new Block { Id = block.Id, BoardId = board.Id, BlockName = name, VisibilityOpening = visible });
						Response.Redirect($"/block/index?" +
							$"Id={idn.GetAscii(block.Id.ToString())}" +
							$"&BoardId={idn.GetAscii(board.Id.ToString())}" +
							$"&BlockName={idn.GetAscii(name)}" +
							$"&VisibilityOpening={idn.GetAscii(visible.ToString())}");
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
					Response.Redirect($"/block/updblock?" +
						$"Id={idn.GetAscii(id)}" +
						$"&BoardId={idn.GetAscii(boardId)}" +
						$"&BlockName={idn.GetAscii(name)}" +
						$"&VisibilityOpening={idn.GetAscii(copy)}");
				}
			}
			else
			{
				_notyf.Error("Заполните все поля");
				Response.Redirect($"/block/updblock?" +
					$"Id={idn.GetAscii(id)}" +
					$"&BoardId={idn.GetAscii(boardId)}" +
					$"&BlockName={idn.GetAscii(name)}" +
					$"&VisibilityOpening={idn.GetAscii(copy)}");
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}