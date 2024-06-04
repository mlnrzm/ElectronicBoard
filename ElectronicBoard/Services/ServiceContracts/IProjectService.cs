using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Проект"
	/// </summary>
	public interface IProjectService
	{
		/// <summary>
		/// Метод для получения списка проектов
		/// </summary>
		/// <returns></returns>
		public Task<List<Project>> GetFullList();

		/// <summary>
		/// Метод для получения списка проектов по Id блока
		/// </summary>
		/// <param name="BlockId"></param>
		/// <returns></returns>
		public Task<List<Project>> GetFilteredList(int BlockId);

		/// <summary>
		/// Метод для получения проекта по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Project> GetElement(Project model);

		/// <summary>
		/// Метод для добавления проекта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Project model);

		/// <summary>
		/// Метод для редактирования проекта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Project model);

		/// <summary>
		/// Метод для удаления проекта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Project model);

		/// <summary>
		/// Метод для прикрепления/открепления ответственного
		/// </summary>
		/// <param name="model"></param>
		/// <param name="project_id"></param>
		/// <returns></returns>
		public Task GetResponsable(Participant model, int project_id);
	}
}
