using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Этап"
	/// </summary>
	public interface IStageService
	{
		/// <summary>
		/// Метод для получения списка этапов
		/// </summary>
		/// <returns></returns>
		public Task<List<Stage>> GetFullList();

		/// <summary>
		/// Метод для получения списка этапов по Id проекта
		/// </summary>
		/// <param name="ProjectId"></param>
		/// <returns></returns>
		public Task<List<Stage>> GetFilteredList(int ProjectId);

		/// <summary>
		/// Метод для получения этапа по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Stage> GetElement(Stage model);

		/// <summary>
		/// Метод для добавления этапа
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Stage model);

		/// <summary>
		/// Метод для редактирования этапа
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Stage model);

		/// <summary>
		/// Метод для удаления этапа
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Stage model);
	}
}
