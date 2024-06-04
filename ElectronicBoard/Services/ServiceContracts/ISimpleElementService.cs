using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Простой элемент"
	/// </summary>
	public interface ISimpleElementService
	{
		/// <summary>
		/// Метод для получения списка элементов
		/// </summary>
		/// <returns></returns>
		public Task<List<SimpleElement>> GetFullList();

		/// <summary>
		/// Метод для получения списка элементов по Id блока
		/// </summary>
		/// <param name="BlockId"></param>
		/// <returns></returns>
		public Task<List<SimpleElement>> GetFilteredList(int BlockId);

		/// <summary>
		/// Метод для получения элемента по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<SimpleElement> GetElement(SimpleElement model);

		/// <summary>
		/// Метод для добавления элемента
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(SimpleElement model);

		/// <summary>
		/// Метод для редактирования элемента
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(SimpleElement model);

		/// <summary>
		/// Метод для удаления элемента
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(SimpleElement model);
	}
}
