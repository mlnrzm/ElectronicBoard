using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IBlockService
	{
		public Task AddOrRemoveElement(Participant participant, int blockId);
		public Task AddOrRemoveElement(Event event_, int blockId);

		// Получение всего списка блоков
		public Task<List<Block>> GetFullList();

		// Получение списка блоков по названию или id доски
		public Task<List<Block>> GetFilteredList(Block model);

		// Получение блока по id или названию
		public Task<Block> GetElement(Block model);

		// Добавление блока
		public Task Insert(Block model);

		// Редактирование данных о блоке
		public Task Update(Block model);

		// Удаление блока
		public Task Delete(Block model);

		public Block CreateModel(Block block);
	}
}
