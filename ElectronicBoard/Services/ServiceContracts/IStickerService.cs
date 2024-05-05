using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IStickerService
	{
		// Получение всего списка стикеров
		public Task<List<Sticker>> GetFullList();

		// Получение стикеров по id элемента (!!!)
		public Task<List<Sticker>> GetFilteredList(string name_element, int id);

		// Получение стикера по id
		public Task<Sticker> GetElement(Sticker model);

		// Добавление стикера
		public Task Insert(Sticker model);

		// Редактирование данных о стикере
		public Task Update(Sticker model);
		// Удаление стикера
		public Task Delete(Sticker model);
		public Sticker CreateModel(Sticker sticker);
	}
}
