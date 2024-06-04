using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Доска"
	/// </summary>
	public class BoardService : IBoardService
    {
        private IBlockService blockService { get; set; }
		public BoardService(IBlockService _blockService)
        {
            blockService = _blockService;
		}

		public BoardService(){}

		/// <summary>
		/// Метод для получения списка досок
		/// </summary>
		/// <returns></returns>
		public async Task<List<Board>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Boards.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка досок участника по Id участника
		/// </summary>
		/// <param name="participantId"></param>
		/// <returns></returns>
		public async Task<List<Board>> GetParticipantBoards(int participantId) 
        {
			if (participantId < 1)
            {
                return new List<Board>();
            }
            else 
            {
				using var context = new ElectronicBoardDatabase();
				var block_part = (await context.BlockParticipants.ToListAsync())
					.Where(rec => rec.ParticipantId == participantId)
					.ToList();
                List<Board> boards = new List<Board>();
                foreach (var block in block_part)
                {
                    Block? this_block = await blockService.GetElement(new Block
                    { Id = block.BlockId });

					if (this_block != null) 
					{
						var board = await GetElement(new Board { Id = this_block.BoardId });
						if (board != null) boards.Add(board); 
					}
                }
                return boards;
			}
		}

		/// <summary>
		/// Метод для получения доски по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Board?> GetElement(Board model)
        {
            if (model == null)
            {
                return null;
            }
            using var context = new ElectronicBoardDatabase();
            var component = await context.Boards
            .FirstOrDefaultAsync(rec => rec.BoardName.Contains(model.BoardName) || rec.Id == model.Id);
            return component != null ? CreateModel(component) : null;
        }

		/// <summary>
		/// Метод для добавления доски
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Board model)
        {
			using var context = new ElectronicBoardDatabase();
			var component = await context.Boards
                .FirstOrDefaultAsync(rec => rec.BoardName.Contains(model.BoardName));
            if (component == null)
            {
                await context.Boards.AddAsync(CreateModel(model, new Board()));
                await context.SaveChangesAsync();

				Board? new_board = await GetElement(new Board
				{
					BoardName = model.BoardName,
					BoardThematics = model.BoardThematics
				});
				if (new_board != null)
				{
					// Добавление блока участников
					await blockService.Insert(new Block
					{
						BoardId = new_board.Id,
						BlockName = "Участники",
						VisibilityOpening = true
					});

					// Добавление блока мероприятий
					await blockService.Insert(new Block
					{
						BoardId = new_board.Id,
						BlockName = "Мероприятия",
						VisibilityOpening = true
					});
				}
				else throw new Exception("Ошибка добавления доски");
			}
            else throw new Exception("Доска с таким названием уже существует");
		}

		/// <summary>
		/// Метод для редактирования доски
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Board model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Boards.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            var elementName = await context.Boards.FirstOrDefaultAsync(rec => rec.BoardName.Contains(model.BoardName));
            if (element == null)
            {
                throw new Exception("Доска не найдена");
            }
            else if (elementName != null && elementName.Id != model.Id) { throw new Exception("Доска с таким названием уже существует"); }
            else
            {
                CreateModel(model, element);
                await context.SaveChangesAsync();
            }
        }

		/// <summary>
		/// Метод для удаления доски
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Board model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Boards.FirstOrDefaultAsync(rec => rec.Id == model.Id);
                if (element != null)
                {
                    if (element.BoardName.Contains("Общая доска"))
                    {
                        throw new Exception("Удалить общую доску невозможно");
                    }
                    else
                    {
                        // Удаление блоков
                        var blocks = (await context.Blocks.ToListAsync())
                            .Where(rec => rec.BoardId == model.Id)
                            .ToList();
                        if (blocks != null)
                        {
                            foreach (var block in blocks)
                            {
                                await blockService.Delete(block);
                            }
                        }
                        context.Boards.Remove(element);
                        await context.SaveChangesAsync();
                    }
                }
                else throw new Exception("Доска не найдена");

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

		/// <summary>
		/// Метод для создания общей доски
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public async Task CreateMainBoard(Participant part)
		{
			List<Board> boards = await GetFullList();

			if (boards.Count == 0)
			{
				using var context = new ElectronicBoardDatabase();
				await context.Boards.AddAsync(CreateModel(new Board
				{
					BoardName = "Общая доска",
					BoardThematics = "общая доска научной группы"
				},
				new Board()));
				await context.SaveChangesAsync();

				Board? new_board = await GetElement(new Board
				{
					BoardName = "Общая доска",
					BoardThematics = "общая доска научной группы"
				});
				if (new_board != null)
				{
					Program.MainBoard = new_board;

					// Добавление блока участников
					await blockService.Insert(new Block
					{
						BoardId = new_board.Id,
						BlockName = "Участники",
						VisibilityOpening = true
					});
					Block? part_block = await blockService.GetElement(new Block
					{
						BoardId = new_board.Id,
						BlockName = "Участники",
						VisibilityOpening = true
					});
					if (part_block != null) await blockService.AddOrRemoveElement(part, part_block.Id);

					// Добавление блока мероприятий
					await blockService.Insert(new Block
					{
						BoardId = new_board.Id,
						BlockName = "Мероприятия",
						VisibilityOpening = true
					});

					// Добавление блока грантов
					await blockService.Insert(new Block
					{
						BoardId = new_board.Id,
						BlockName = "Гранты",
						VisibilityOpening = true
					});

					// Добавление блока проектов
					await blockService.Insert(new Block
					{
						BoardId = new_board.Id,
						BlockName = "Проекты",
						VisibilityOpening = true
					});
				}
				else throw new Exception("Ошибка создания общей доски");
			}
		}

		public Board CreateModel(Board model, Board board)
        {
            board.BoardName = model.BoardName;
            board.BoardThematics = model.BoardThematics;
            board.Blocks = model.Blocks;
            return board;
        }
        private static Board CreateModel(Board board)
        {
            return new Board
            {
                Id = board.Id,
                BoardName = board.BoardName,
                BoardThematics = board.BoardThematics,
                Blocks = board.Blocks
            };
        }
    }
}