using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IProjectService
	{
		// Получение всего списка проектов
		public Task<List<Project>> GetFullList();
		// Получение проектов по id блока
		public Task<List<Project>> GetFilteredList(int BlockId);
		// Получение проекта по id или названию
		public Task<Project> GetElement(Project model);
		// Добавление проекта
		public Task Insert(Project model);
		// Редактирование данных о проекте
		public Task Update(Project model);
		// Удаление проекта
		public Task Delete(Project model);
		// Привязка и отвязка ответственных за проект
		public Task GetResponsable(Participant model, int project_id);
	}
}
