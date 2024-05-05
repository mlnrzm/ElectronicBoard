using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IGrantService
	{
		// Получение всего списка грантов
		public Task<List<Grant>> GetFullList();

		// Получение грантов по id блока (!!!)
		public Task<List<Grant>> GetFilteredList(int BlockId);

		// Получение гранта по id или названию
		public Task<Grant> GetElement(Grant model);

		// Добавление гранта
		public Task Insert(Grant model);

		// Редактирование данных о гранте
		public Task Update(Grant model);

		// Удаление гранта
		public Task Delete(Grant model);
		// Привязка и отвязка участника от гранта 
		public Task GetParticipant(Participant model, int grant_id);
		// Получение участников гранта
		public Task<List<Participant>> GetParticipants(int grantId);
	}
}
