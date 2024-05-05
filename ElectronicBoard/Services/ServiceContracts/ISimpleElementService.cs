using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface ISimpleElementService
	{
		// Получение всего списка элементов
		public Task<List<SimpleElement>> GetFullList();

		// Получение элементов по id блока
		public Task<List<SimpleElement>> GetFilteredList(int BlockId);

		// Получение элемента по id или названию
		public Task<SimpleElement> GetElement(SimpleElement model);

		// Добавление элемента
		public Task Insert(SimpleElement model);

		// Редактирование данных 
		public Task Update(SimpleElement model);

		// Удаление элемента
		public Task Delete(SimpleElement model);
	}
}
