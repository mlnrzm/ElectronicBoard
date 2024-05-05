using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IStageService
	{
		// Получение всего списка этапов
		public Task<List<Stage>> GetFullList();
		// Получение этапов по id проекта
		public Task<List<Stage>> GetFilteredList(int ProjectId);
		// Получение этапа по id или названию
		public Task<Stage> GetElement(Stage model);
		// Добавление этапа
		public Task Insert(Stage model);
		// Редактирование данных об этапе
		public Task Update(Stage model);
		// Удаление этапа
		public Task Delete(Stage model);
	}
}
