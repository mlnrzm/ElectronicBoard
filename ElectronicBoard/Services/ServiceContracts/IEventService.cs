using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Мероприятие"
	/// </summary>
	public interface IEventService
	{
		/// <summary>
		/// Метод для получения списка мероприятий
		/// </summary>
		/// <returns></returns>
		public Task<List<Event>> GetFullList();

		/// <summary>
		/// Метод для получения списка мероприятий по названию или Id блока
		/// </summary>
		/// <param name="model"></param>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public Task<List<Event>> GetFilteredList(int? blockId);

		/// <summary>
		/// Метод для получения мероприятия по Id или наименованию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Event?> GetElement(Event model);

		/// <summary>
		/// Метод для добавления мероприятия
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Event model);

		/// <summary>
		/// Метод для редактирования мероприятия
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Event model);

		/// <summary>
		/// Метод для удаления мероприятия
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Event model);
	}
}
