using ElectronicBoard.Models;
using File = ElectronicBoard.Models.File;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IFileService
	{
		// Получение всего списка файлов
		public Task<List<File>> GetFullList();

		// Получение файлов по id элемента (!!!)
		public Task<List<File>> GetFilteredList(string name_element, int id);

		// Получение файла по id или названию
		public Task<File> GetElement(File model);

		// Добавление файла
		public Task Insert(File model);

		// Редактирование данных о файле
		public Task Update(File model);

		// Удаление элемента
		public Task Delete(File model);
		public File CreateModel(File file);
	}
}
