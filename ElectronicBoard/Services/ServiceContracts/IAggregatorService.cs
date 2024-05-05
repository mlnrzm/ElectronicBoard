using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IAggregatorService
	{
		// Получение всего списка агрегаторов
		public Task<List<Aggregator>> GetFullList();

		// Получение агрегаторов по id статьи
		public Task<List<Aggregator>> GetFilteredList(int ArticleId);

		// Получение агрегатора по id или наименованию
		public Task<Aggregator> GetElement(Aggregator model);

		// Добавление агрегатора
		public Task Insert(Aggregator model);

		// Редактирование данных об агрегаторе
		public Task Update(Aggregator model);

		// Удаление агрегатора
		public Task Delete(Aggregator model);
		public Aggregator CreateModel(Aggregator model, Aggregator aggregator);

	}
}
