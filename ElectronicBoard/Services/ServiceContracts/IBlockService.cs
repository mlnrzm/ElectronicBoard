using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Блок"
	/// </summary>
	public interface IBlockService
	{
		/// <summary>
		/// Метод для получения списка блоков
		/// </summary>
		/// <returns></returns>
		public Task<List<Block>> GetFullList();

		/// <summary>
		/// Метод для получения списка блоков (по Id доски)
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<List<Block>> GetFilteredList(Block model);

		/// <summary>
		/// Метод для получения блока по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Block?> GetElement(Block model);

		/// <summary>
		/// Метод для добавления блока
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Block model);

		/// <summary>
		/// Метод для редактирования блока
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Block model);

		/// <summary>
		/// Метод для удаления блока
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Block model);

		/// <summary>
		/// Метод для добавления/удаления участника из блока
		/// </summary>
		/// <param name="participant"></param>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public Task AddOrRemoveElement(Participant participant, int blockId);

		/// <summary>
		/// Метод для добавления/удаления мероприятия из блока
		/// </summary>
		/// <param name="event_"></param>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public Task AddOrRemoveElement(Event event_, int blockId);

		/// <summary>
		/// Метод для создания модели блока
		/// </summary>
		/// <param name="block"></param>
		/// <returns></returns>
		public Block CreateModel(Block block);
	}
}
