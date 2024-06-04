using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Грант"
	/// </summary>
	public interface IGrantService
	{
		/// <summary>
		/// Метод для получения списка грантов
		/// </summary>
		/// <returns></returns>
		public Task<List<Grant>> GetFullList();

		/// <summary>
		/// Метод для получения списка грантов по Id блока
		/// </summary>
		/// <param name="BlockId"></param>
		/// <returns></returns>
		public Task<List<Grant>> GetFilteredList(int BlockId);

		/// <summary>
		/// Метод для получения гранта по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Grant?> GetElement(Grant model);

		/// <summary>
		/// Метод для добавления гранта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Grant model);

		/// <summary>
		/// Метод для редактирования гранта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Grant model);

		/// <summary>
		/// Метод для удаления гранта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Grant model);

		/// <summary>
		/// Метод для прикрепления/открепления участника от гранта
		/// </summary>
		/// <param name="model"></param>
		/// <param name="grant_id"></param>
		/// <returns></returns>
		public Task GetParticipant(Participant model, int grant_id);

		/// <summary>
		/// Метод для получения списка участников гранта
		/// </summary>
		/// <param name="grantId"></param>
		/// <returns></returns>
		public Task<List<Participant>> GetParticipants(int grantId);
	}
}
