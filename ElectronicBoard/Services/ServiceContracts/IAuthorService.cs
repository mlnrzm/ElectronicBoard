using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IAuthorService
	{
		// Получение всего списка авторов
		public Task<List<Author>> GetFullList();
		// Получение авторов по id статьи
		public Task<List<Author>> GetFilteredList(int ArticleId);

		// Получение автора по id или ФИО
		public Task<Author> GetElement(Author model);

		// Добавление автора
		public Task Insert(Author model);

		// Редактирование данных об авторе
		public Task Update(Author model);

		// Удаление автора
		public Task Delete(Author model);
		public Author CreateModel(Author model, Author author);
	}
}
