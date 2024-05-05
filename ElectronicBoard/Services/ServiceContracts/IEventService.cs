using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IEventService
	{
		// Получение всего списка событий
		public Task<List<Event>> GetFullList();

		// Получение событий по имени или по id блока
		public Task<List<Event>> GetFilteredList(Event? model, int? blockId);

		// Получение события по id или наименованию
		public Task<Event> GetElement(Event model);

		// Добавление события
		public Task Insert(Event model);

		// Редактирование данных о событии
		public Task Update(Event model);

		// Удаление события
		public Task Delete(Event model);
	}
}
