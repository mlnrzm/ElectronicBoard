using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Стикер"
	/// </summary>
	public interface IStickerService
	{
		/// <summary>
		/// Метод для получения списка стикеров
		/// </summary>
		/// <returns></returns>
		public Task<List<Sticker>> GetFullList();

		/// <summary>
		/// Метод для получения списка стикеров по названию и Id элемента
		/// </summary>
		/// <param name="name_element"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public Task<List<Sticker>> GetFilteredList(string name_element, int id);

		/// <summary>
		/// Метод для получения стикера по Id
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Sticker> GetElement(Sticker model);

		/// <summary>
		/// Метод для добавления стикера
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Sticker model);

		/// <summary>
		/// Метод для редактирования стикера
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Sticker model);

		/// <summary>
		/// Метод для удаления стикера
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Sticker model);

		/// <summary>
		/// Метод для создания модели стикера
		/// </summary>
		/// <param name="sticker"></param>
		/// <returns></returns>
		public Sticker CreateModel(Sticker sticker);
	}
}
