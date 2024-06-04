using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Файл"
	/// </summary>
	public interface IFileService
	{
		/// <summary>
		/// Метод для получения списка файлов
		/// </summary>
		/// <returns></returns>
		public Task<List<File>> GetFullList();

		/// <summary>
		/// Метод для получения списка файлов по названию и Id элемента, к которому прикреплен файл
		/// </summary>
		/// <param name="name_element"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public Task<List<File>> GetFilteredList(string name_element, int id);

		/// <summary>
		/// Метод для получения файла по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<File?> GetElement(File model);

		/// <summary>
		/// Метод для добавления файла
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(File model);

		/// <summary>
		/// Метод для редактирования файла
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(File model);

		/// <summary>
		/// Метод для удаления файла
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(File model);

		/// <summary>
		/// Метод для создания модели файла
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public File CreateModel(File file);
	}
}
