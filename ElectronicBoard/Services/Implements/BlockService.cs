using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Блок"
	/// </summary>
	public class BlockService : IBlockService
    {
        private ISimpleElementService simpleElementService { get; set; }

		public BlockService(ISimpleElementService _simpleElementService)
		{
            simpleElementService = _simpleElementService;
        }

		public BlockService(){}

		/// <summary>
		/// Метод для получения списка блоков
		/// </summary>
		/// <returns></returns>
		public async Task<List<Block>> GetFullList()
        {
            using var context = new ElectronicBoardDatabase();
            return (await context.Blocks.ToListAsync())
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения списка блоков (по Id доски)
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<List<Block>> GetFilteredList(Block model)
        {
            if (model == null)
            {
                return new List<Block>();
            }
            using var context = new ElectronicBoardDatabase();
            if(model.BlockName == null)
				return (await context.Blocks.ToListAsync())
                .Where(rec => rec.BoardId == model.BoardId)
                .Select(CreateModel)
                .ToList();

			return (await context.Blocks.ToListAsync())
            .Where(rec => rec.BlockName.Contains(model.BlockName) || rec.BoardId == model.BoardId)
            .Select(CreateModel)
            .ToList();
        }

		/// <summary>
		/// Метод для получения блока по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<Block?> GetElement(Block model)
        {
            if (model == null)
            {
                return null;
            }

            using var context = new ElectronicBoardDatabase();
            var component = await context.Blocks
					.FirstOrDefaultAsync(rec => rec.BlockName.Contains(model.BlockName) || rec.Id == model.Id);

			if (!string.IsNullOrEmpty(model.BlockName) && model.BoardId != 0)
            {
                component = await context.Blocks
                        .FirstOrDefaultAsync(rec => rec.BlockName.Contains(model.BlockName) && rec.BoardId == model.BoardId);
            }
			return component != null ? CreateModel(component) : null;
		}

		/// <summary>
		/// Метод для добавления блока
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(Block model)
        {
            using var context = new ElectronicBoardDatabase();
            if (await CheckBlockName(model)) 
            {
				await context.Blocks.AddAsync(CreateModel(model, new Block()));
				await context.SaveChangesAsync();
			}
            else throw new Exception("Введённое наименование блока недопустимо");
		}

		/// <summary>
		/// Метод для редактирования блока
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Update(Block model)
        {
            using var context = new ElectronicBoardDatabase();
            var element = await context.Blocks.FirstOrDefaultAsync(rec => rec.Id == model.Id);
            if (element == null)
            {
                throw new Exception("Блок не найден");
            }
            if (await CheckBlockName(model))
            {
				CreateModel(model, element);
				await context.SaveChangesAsync();
			}
			else throw new Exception("Введённое наименование блока недопустимо");
        }

		/// <summary>
		/// Метод для удаления блока
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(Block model)
        {
            using var context = new ElectronicBoardDatabase();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var element = await context.Blocks.FirstOrDefaultAsync(rec => rec.Id == model.Id);
				if (element != null)
                {
                    if (element.BlockName.Contains("Участники") || element.BlockName.Contains("Мероприятия")
                        || element.BlockName.Contains("Гранты") || element.BlockName.Contains("Проекты"))
                    {
                        throw new Exception("Удалить предопределённый блок невозможно");
                    }
                    else
                    {
                        // Удаление простых элементов
                        var simps = (await context.SimpleElements.ToListAsync())
                            .Where(rec => rec.BlockId == model.Id)
                            .ToList();
                        if (simps != null)
                        {
                            foreach (var simp in simps)
                            {
                                await simpleElementService.Delete(simp);
                            }
                        }
                        context.Blocks.Remove(element);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new Exception("Блок не найден");
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

		/// <summary>
		/// Метод для проверки имени блока
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		private async Task<bool> CheckBlockName(Block model) 
        {
			using var context = new ElectronicBoardDatabase();

			var board = await context.Boards
					.FirstOrDefaultAsync(rec => rec.Id == model.BoardId);
			var block = await context.Blocks
		            .FirstOrDefaultAsync(rec => rec.BlockName.Contains(model.BlockName) && rec.BoardId == model.BoardId);

            // Имена блоков не могут быть одинаковыми
            if (board != null && block == null)
            {
                // Нельзя добавлять блоки с предопределенными названиями
                if (!board.BoardName.Contains("Общая доска"))
                {
                    if (model.BlockName.Contains("Гранты") || model.BlockName.Contains("Проекты")) return false;
                }
            }
            else if (board != null && block != null && block.Id == model.Id) return true;
            else return false;

            return true;
		}

		/// <summary>
		/// Метод для добавления/удаления участника из блока
		/// </summary>
		/// <param name="participant"></param>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public async Task AddOrRemoveElement(Participant participant, int blockId)
		{
			using var context = new ElectronicBoardDatabase();
			var part = await context.Participants.FirstOrDefaultAsync(rec => rec.Id == participant.Id);
			var block = await context.Blocks.FirstOrDefaultAsync(rec => rec.Id == blockId);

			if (part != null && block != null)
			{
				var block_part = await context.BlockParticipants.FirstOrDefaultAsync(rec => rec.ParticipantId == part.Id && rec.BlockId == block.Id);
				if (block_part == null)
				{
					await context.BlockParticipants.AddAsync(new BlockParticipant
					{
						ParticipantId = participant.Id,
						BlockId = blockId
					});
					await context.SaveChangesAsync();
				}
				else
				{
					Board? board = await context.Boards.FirstOrDefaultAsync(rec => rec.Id == block.BoardId);

					if (board != null && board.BoardName.Contains("Общая доска"))
					{
						throw new Exception("Покинуть общую доску невозможно");
					}
					else
					{
						context.BlockParticipants.Remove(block_part);
						await context.SaveChangesAsync();
					}
				}
			}
			else throw new Exception("Участник или блок не найдены");
		}

		/// <summary>
		/// Метод для добавления/удаления мероприятия из блока
		/// </summary>
		/// <param name="event_"></param>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public async Task AddOrRemoveElement(Event event_, int blockId)
		{
			using var context = new ElectronicBoardDatabase();
			var ev = await context.Events.FirstOrDefaultAsync(rec => rec.Id == event_.Id);
			var block = await context.Blocks.FirstOrDefaultAsync(rec => rec.Id == blockId);

			if (ev != null && block != null)
			{
				var block_event = await context.BlockEvents.FirstOrDefaultAsync(rec => rec.EventId == ev.Id && rec.BlockId == block.Id);
				if (block_event == null)
				{
					await context.BlockEvents.AddAsync(new BlockEvent
					{
						EventId = ev.Id,
						BlockId = blockId
					});
					await context.SaveChangesAsync();
				}
				else
				{
					context.BlockEvents.Remove(block_event);
					await context.SaveChangesAsync();
				}
			}
			else throw new Exception("Мероприятие или блок не найдены");
		}

		/// <summary>
		/// Метод для создания модели блока
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		public Block CreateModel(Block model, Block block)
        {
            block.BlockName = model.BlockName;
            block.BoardId = model.BoardId;
            block.VisibilityOpening = model.VisibilityOpening;

            block.BlockSimpleElements = model.BlockSimpleElements;
            block.BlockParticipants = model.BlockParticipants;
            block.BlockEvents = model.BlockEvents;
            block.BlockGrants = model.BlockGrants;
            block.BlockProjects = model.BlockProjects;
            block.Board = model.Board;

            return block;
        }
        public Block CreateModel(Block block)
        {
            return new Block
            {
                Id = block.Id,

                BlockName = block.BlockName,
                BoardId = block.BoardId,
                VisibilityOpening = block.VisibilityOpening,

                BlockSimpleElements = block.BlockSimpleElements,
                BlockParticipants = block.BlockParticipants,
                BlockEvents = block.BlockEvents,
                BlockGrants = block.BlockGrants,
                BlockProjects = block.BlockProjects,
                Board = block.Board
            };
        }
    }
}
