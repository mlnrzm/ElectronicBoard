using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Статья"
	/// </summary>
	public interface IArticleService
	{
		/// <summary>
		/// Метод для получения списка статей
		/// </summary>
		/// <returns></returns>
		public Task<List<Article>> GetFullList();

		/// <summary>
		/// Метод для получения списка статей по Id события
		/// </summary>
		/// <param name="event_id"></param>
		/// <returns></returns>
		public Task<List<Article>> GetFilteredList(int event_id);

		/// <summary>
		/// Метод для получения статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Article?> GetElement(Article model);

		/// <summary>
		/// Метод для добавления статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Article model);

		/// <summary>
		/// Метод для редактирования статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Article model);

		/// <summary>
		/// Метод для удаления статьи
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Article model);

		/// <summary>
		/// Метод для прикрепления/открепления агрегатора
		/// </summary>
		/// <param name="model"></param>
		/// <param name="article_id"></param>
		/// <returns></returns>
		public Task GetAggregator(Aggregator model, int article_id);

		/// <summary>
		/// Метод для прикрепления/открепления автора
		/// </summary>
		/// <param name="model"></param>
		/// <param name="article_id"></param>
		/// <returns></returns>
		public Task GetAuthor(Author model, int article_id);

		/// <summary>
		/// Метод для получения списка статей автора
		/// </summary>
		/// <param name="author_id"></param>
		/// <returns></returns>
		public Task<List<Article>> GetArticlesAuthor(int author_id);

		/// <summary>
		/// Метод для создания модели статьи
		/// </summary>
		/// <param name="article"></param>
		/// <returns></returns>
		public Article CreateModel(Article article);
	}
}
