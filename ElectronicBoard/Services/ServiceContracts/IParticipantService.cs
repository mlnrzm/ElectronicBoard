using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IParticipantService
	{
		// Получение всего списка участников
		public Task<List<Participant>> GetFullList();
		// Поиск участника для входа
		public Task<Participant> Enter(string login, string password);

		// Получение участников по имени или id блока
		public Task<List<Participant>> GetFilteredList(Participant? model, int? blockId);

		// Получение участника по id или ФИО
		public Task<Participant> GetElement(Participant model);

		// Добавление участника
		public Task Insert(Participant model);

		// Редактирование данных об участнике
		public Task Update(Participant model);

		// Удаление участника
		public Task Delete(Participant model);
		// Удаление участника
		public Task UpdRaiting(int partId);
	}
}
