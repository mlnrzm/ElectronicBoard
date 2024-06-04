using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Агрегатор"
	/// </summary>
	public interface IAggregatorService
	{
		/// <summary>
		/// Метод для получения списка агрегаторов
		/// </summary>
		/// <returns></returns>
		public Task<List<Aggregator>> GetFullList();

		/// <summary>
		/// Метод для получения списка агрегаторов по Id статьи
		/// </summary>
		/// <param name="ArticleId"></param>
		/// <returns></returns>
		public Task<List<Aggregator>> GetFilteredList(int ArticleId);

		/// <summary>
		/// Метод для получения агрегатора по Id или наименованию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Aggregator?> GetElement(Aggregator model);

		/// <summary>
		/// Метод для добавления агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Aggregator model);

		/// <summary>
		/// Метод для редактирования агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Aggregator model);

		/// <summary>
		/// Метод для удаления агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Aggregator model);

		/// <summary>
		/// Метод для создания модели агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Aggregator CreateModel(Aggregator model, Aggregator aggregator);
	}
}
