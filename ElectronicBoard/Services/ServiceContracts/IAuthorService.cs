using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Автор"
	/// </summary>
	public interface IAuthorService
	{
		/// <summary>
		/// Метод для получения списка авторов
		/// </summary>
		/// <returns></returns>
		public Task<List<Author>> GetFullList();

		/// <summary>
		/// Метод для получения списка авторов по Id статьи
		/// </summary>
		/// <param name="ArticleId"></param>
		/// <returns></returns>
		public Task<List<Author>> GetFilteredList(int ArticleId);

		/// <summary>
		/// Метод для получения автора по Id или ФИО
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Author?> GetElement(Author model);

		/// <summary>
		/// Метод для добавления автора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Author model);

		/// <summary>
		/// Метод для редактирования автора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Author model);

		/// <summary>
		/// Метод для удаления автора
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Author model);

		/// <summary>
		/// Метод для создания модели автора
		/// </summary>
		/// <param name="model"></param>
		/// <param name="author"></param>
		/// <returns></returns>
		public Author CreateModel(Author model, Author author);
	}
}
