using AspNetCoreHero.ToastNotification.Abstractions;
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
	/// Контроллер, обрабатывающий запросы касающиеся досок
	/// </summary>
	[Authorize]
	public class BoardController : Controller
    {
		private readonly ILogger<BoardController> _logger;
		private readonly UserManager<IdentityUser<int>> _userManager;
		private readonly IBlockService blockService;
		private readonly IBoardService boardService;
		private readonly IParticipantService participantService;

		private readonly INotyfService _notyf;
		private readonly IdnMapping idn;

		public BoardController(UserManager<IdentityUser<int>> userManager, ILogger<BoardController> logger,
			INotyfService notyf, IBlockService _blockService, IBoardService _boardService, IParticipantService _participantService)
        {
            _logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			boardService = _boardService;
			participantService = _participantService;
			_userManager = userManager;
			idn = new IdnMapping();
		}

		/// <summary>
		/// Метод для отображения страницы с информацией о доске
		/// </summary>
		/// <param name="board"></param>
		/// <returns></returns>
		public async Task<IActionResult> Index(Board board)
        {
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Board? find_board = await boardService.GetElement(new Board { Id = board.Id, BoardName = idn.GetUnicode(board.BoardName) });

				if (find_board != null)
				{
					List<Block> added_blocks = new List<Block>();
					foreach (var block in await blockService.GetFilteredList(new Block { BoardId = find_board.Id })) { added_blocks.Add(block); }

					return View(new Board
					{
						Id = find_board.Id,
						BoardName = find_board.BoardName,
						BoardThematics = find_board.BoardThematics,
						Blocks = added_blocks
					});
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
		/// Метод для отображения страницы добавления доски
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddBoard() 
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				return View();
			}
			catch (Exception ex)
			{
				_notyf.Error(ex.Message);
				return Redirect("javascript: history.go(-1)");
			}
		}
		[HttpPost]
		public async Task AddBoard(string name, string thematic)
		{
			if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(thematic))
			{
				try
				{
					// Добавление и отображение доски
					await boardService.Insert(new Board { BoardName = name, BoardThematics = thematic });
					Board? new_board = await boardService.GetElement(new Board
					{
						BoardName = name,
						BoardThematics = thematic
					});
					// Добавление участника на доску: найти блок с участниками новой доски и прикрепить участника
					Block? part_block = await blockService.GetElement(new Block { BoardId = new_board.Id, BlockName = "Участники" });

					IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
					Participant? part = await participantService.GetElement(new Participant { IdentityId = UserId.Id });

					await blockService.AddOrRemoveElement(part, part_block.Id);

					Response.Redirect($"/board/index?" +
						$"Id={idn.GetAscii(new_board.Id.ToString())}" +
						$"&BoardName={idn.GetAscii(new_board.BoardName)}" +
						$"&BoardThematics={idn.GetAscii(new_board.BoardThematics)}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/board/addboard");
				}				
			}
			else
			{
				_notyf.Error("Введите название доски и тематику");
				Response.Redirect($"/board/addboard");
			}
		}

		/// <summary>
		/// Метод для отображения страницы редактирования доски
		/// </summary>
		/// <param name="board"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> UpdBoard(Board board)
		{
			try
			{
				IdentityUser<int>? UserId = await _userManager.GetUserAsync(HttpContext.User);
				Participant? activeUser = await participantService.GetElement(new Participant { IdentityId = UserId.Id });
				ViewBag.ActivePart = activeUser;

				List<Board> activeBoards = await boardService.GetParticipantBoards(activeUser.Id);
				ViewBag.ActiveBoards = activeBoards;

				Board? find_board = await boardService.GetElement(new Board { Id = board.Id, BoardName = idn.GetUnicode(board.BoardName) });

				if (find_board != null)
				{
					return View(find_board);
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
		public async Task UpdBoard(string id, string name, string thematic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(thematic))
			{
				try
				{
					Board? find_board = await boardService.GetElement(new Board { Id = Convert.ToInt32(id) });
					if (find_board != null)
					{
						// Редактирование и отображение доски
						await boardService.Update(new Board { Id = Convert.ToInt32(id), BoardName = name, BoardThematics = thematic });
						Response.Redirect($"/board/index?" +
							$"Id={idn.GetAscii(id)}" +
							$"&BoardName={idn.GetAscii(name)}" +
							$"&BoardThematics={idn.GetAscii(thematic)}");
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
					Response.Redirect($"/board/updboard?" +
						$"Id={idn.GetAscii(id)}" +
						$"&BoardName={idn.GetAscii(name)}" +
						$"&BoardThematics={idn.GetAscii(thematic)}");
				}				
			}
			else
			{
				_notyf.Error("Введите название доски и тематику");
				Response.Redirect($"/board/updboard?" +
							$"Id={idn.GetAscii(id)}" +
							$"&BoardName={idn.GetAscii(name)}" +
							$"&BoardThematics={idn.GetAscii(thematic)}");
			}
		}

		/// <summary>
		/// Метод для удаления доски
		/// </summary>
		/// <param name="boardId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task DeleteBoard(string boardId)
		{
			if (!string.IsNullOrEmpty(boardId))
			{
				Board? find_board = await boardService.GetElement(new Board { Id = Convert.ToInt32(boardId) });
				if (find_board != null)
				{
					try
					{
						await boardService.Delete(new Board { Id = find_board.Id });
						Response.Redirect($"/board/index?BoardName={idn.GetAscii("Общая доска")}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/board/index?Id={idn.GetAscii(find_board.Id.ToString())}");
					}
				}
				else
				{
					_notyf.Error("Доска не найдена");
					Response.Redirect($"/board/index?BoardName={idn.GetAscii("Общая доска")}");
				}
			}
			else
			{
				_notyf.Error("Ошибка");
				Response.Redirect("javascript: history.go(-1)");
			}
		}

		/// <summary>
		/// Метод для выхода участника из доски
		/// </summary>
		/// <param name="boardId"></param>
		/// <param name="partId"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task ExitBoard(string boardId, string partId)
		{
			if (!string.IsNullOrEmpty(boardId) && !string.IsNullOrEmpty(partId))
			{
				Board? find_board = await boardService.GetElement(new Board { Id = Convert.ToInt32(boardId) });
				Participant? find_participant = await participantService.GetElement(new Participant { Id = Convert.ToInt32(partId) });
				if (find_board != null && find_participant != null)
				{

					try
					{
						// Метод отвязки участника от доски
						Block? block_part = await blockService.GetElement(new Block { BlockName = "Участники", BoardId = find_board.Id });
						Participant? part = await participantService.GetElement(new Participant { Id = find_participant.Id });
						if (block_part != null && part != null)
						{
							await blockService.AddOrRemoveElement(part, block_part.Id);
							Response.Redirect($"/board/index?BoardName={idn.GetAscii("Общая доска")}");
						}
						else
						{
							_notyf.Error("Доска не найдена");
							Response.Redirect($"/board/index?BoardName={idn.GetAscii("Общая доска")}");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/board/index?Id={idn.GetAscii(find_board.Id.ToString())}");
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
				_notyf.Error("Доска не найдена");
				Response.Redirect($"/board/index?BoardName={idn.GetAscii("Общая доска")}");
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
