using AspNetCoreHero.ToastNotification.Abstractions;
using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Diagnostics;

namespace ElectronicBoard.Controllers
{
	public class BoardController : Controller
    {
		private readonly ILogger<BoardController> _logger;
		private readonly IBlockService blockService;
		private readonly IBoardService boardService;
		private readonly IParticipantService participantService;

		private readonly INotyfService _notyf;

		public BoardController(ILogger<BoardController> logger,
			INotyfService notyf, IBlockService _blockService, IBoardService _boardService, IParticipantService _participantService)
        {
            _logger = logger;
			_notyf = notyf;
			blockService = _blockService;
			boardService = _boardService;
			participantService = _participantService;
		}

		// ����������� �����
        public async Task<IActionResult> Index(Board board)
        {
			Board find_board = await boardService.GetElement(new Board { Id = board.Id, BoardName = board.BoardName, BoardThematics = board.BoardThematics });

			List<Block> added_blocks = new List<Block>();
			foreach (var block in await blockService.GetFilteredList(new Block { BoardId = find_board.Id })) { added_blocks.Add(block); }

			return View( new Board
			{
				Id = find_board.Id,
				BoardName = find_board.BoardName,
				BoardThematics = find_board.BoardThematics,
				Blocks = added_blocks
			});
		}

		// ���������� �����
		[HttpGet]
		public IActionResult AddBoard() 
		{
			return View();
		}
		[HttpPost]
		public async Task AddBoard(string name, string thematic)
		{
			if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(thematic))
			{
				try
				{
					// ���������� � ����������� �����
					await boardService.Insert(new Board { BoardName = name, BoardThematics = thematic });
					Board new_board = await boardService.GetElement(new Board
					{
						BoardName = name,
						BoardThematics = thematic
					});
					// ���������� ��������� �� �����: ����� ���� � ����������� ����� ����� � ���������� ���������
					Block part_block = await blockService.GetElement(new Block { BoardId = new_board.Id, BlockName = "���������" });
					await blockService.AddOrRemoveElement(Program.Participant, part_block.Id);
					Program.Boards = await boardService.GetParticipantBoards(Program.Participant.Id);

					Response.Redirect($"/board/index?" +
						$"Id={new_board.Id}" +
						$"&BoardName={new_board.BoardName}" +
						$"&BoardThematics={new_board.BoardThematics}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/board/addboard");
				}				
			}
			else
			{
				_notyf.Error("������� �������� ����� � ��������");
				Response.Redirect($"/board/addboard");
			}
		}

		// �������������� �����
		[HttpGet]
		public async Task<IActionResult> UpdBoard(Board board)
		{
			Board find_board = await boardService.GetElement(new Board { Id = board.Id, BoardName = board.BoardName, BoardThematics = board.BoardThematics });
			return View(find_board);
		}
		[HttpPost]
		public async Task UpdBoard(string id, string name, string thematic)
		{
			if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(thematic))
			{
				try
				{
					// �������������� � ����������� �����
					await boardService.Update(new Board { Id = Convert.ToInt32(id), BoardName = name, BoardThematics = thematic });
					Response.Redirect($"/board/index?" +
						$"Id={id}" +
						$"&BoardName={name}" +
						$"&BoardThematics={thematic}");
				}
				catch (Exception ex)
				{
					_notyf.Error(ex.Message);
					Response.Redirect($"/board/updboard?" +
						$"Id={id}" +
						$"&BoardName={name}" +
						$"&BoardThematics={thematic}");
				}				
			}
			else
			{
				_notyf.Error("������� �������� ����� � ��������");
				Response.Redirect($"/board/updboard?" +
							$"Id={id}" +
							$"&BoardName={name}" +
							$"&BoardThematics={thematic}");
			}
		}

		// �������� �����
		[HttpGet]
		public async Task DeleteBoard(string boardId)
		{
			if (!string.IsNullOrEmpty(boardId))
			{
				int board_id;
				bool isNumeric_boardId = int.TryParse(boardId, out board_id);
				if (isNumeric_boardId)
				{
					try
					{
						await boardService.Delete(new Board { Id = board_id });
						Response.Redirect($"/board/index?BoardName={"����� �����"}");
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/board/index?Id={board_id}");
					}
				}
				else
				{
					_notyf.Error("����� �� �������");
					Response.Redirect($"/board/index?BoardName={"����� �����"}");
				}
			}
		}

		// �������� �����
		[HttpGet]
		public async Task ExitBoard(string boardId, string partId)
		{
			if (!string.IsNullOrEmpty(boardId) && !string.IsNullOrEmpty(partId))
			{
				int board_id;
				int part_id;
				bool isNumeric_boardId = int.TryParse(boardId, out board_id);
				bool isNumeric_partId = int.TryParse(partId, out part_id);
				if (isNumeric_boardId && isNumeric_partId)
				{
					try
					{
						// ����� ������� ��������� �� �����
						Block block_part = await blockService.GetElement(new Block { BlockName = "���������", BoardId = board_id });
						Participant part = await participantService.GetElement(new Participant { Id = part_id });
						if (block_part != null && part != null)
						{
							await blockService.AddOrRemoveElement(part, block_part.Id);
							Response.Redirect($"/board/index?BoardName={"����� �����"}");
						}
						else
						{
							_notyf.Error("����� �� �������");
							Response.Redirect($"/board/index?BoardName={"����� �����"}");
						}
					}
					catch (Exception ex)
					{
						_notyf.Error(ex.Message);
						Response.Redirect($"/board/index?Id={board_id}");
					}
				}
				else
				{
					_notyf.Error("����� �� �������");
					Response.Redirect($"/board/index?BoardName={"����� �����"}");
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
