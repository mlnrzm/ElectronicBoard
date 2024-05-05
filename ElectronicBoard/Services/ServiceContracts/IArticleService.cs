using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IArticleService
	{
		// Получение всего списка статей
		public Task<List<Article>> GetFullList();
		// Получение статей по имени или id события
		public Task<List<Article>> GetFilteredList(int event_id);

		// Получение статьи по id или наименованию
		public Task<Article> GetElement(Article model);

		// Добавление статьи
		public Task Insert(Article model);

		// Редактирование данных о статье
		public Task Update(Article model);

		// Удаление статьи
		public Task Delete(Article model);

		// Привязка и отвязка агрегатора от статьи
		public Task GetAggregator(Aggregator model, int article_id);

		// Привязка и отвязка автора от статьи
		public Task GetAuthor(Author model, int article_id);

		// Получение статей автора
		public Task<List<Article>> GetArticlesAuthor(int author_id);
		public Article CreateModel(Article article);
	}
}
