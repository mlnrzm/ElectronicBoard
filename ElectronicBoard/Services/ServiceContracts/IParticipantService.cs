using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Участник"
	/// </summary>
	public interface IParticipantService
	{
		/// <summary>
		/// Метод для получения списка участников
		/// </summary>
		/// <returns></returns>
		public Task<List<Participant>> GetFullList();

		/// <summary>
		/// Метод для получения участников по Id блока
		/// </summary>
		/// <param name="blockId"></param>
		/// <returns></returns>
		public Task<List<Participant>> GetFilteredList(int? blockId);

		/// <summary>
		/// Метод для проверки логина/пароля участника
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public Task<Participant?> Enter(string login, string password);

		/// <summary>
		/// Метод для получения участника по Id или ФИО
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Participant?> GetElement(Participant model);

		/// <summary>
		/// Метод для добавления участника
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Participant model);

		/// <summary>
		/// Метод для редактирования участника
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Participant model);

		/// <summary>
		/// Метод для удаления участника
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Participant model);

		/// <summary>
		/// Метод для регистрации участника в приложении
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public Task Register(Participant part);

		/// <summary>
		/// Метод для обновления рейтинга
		/// </summary>
		/// <param name="partId"></param>
		/// <returns></returns>
		public Task UpdRaiting(int partId);

		/// <summary>
		/// Метод для создания тестового участника
		/// </summary>
		/// <param name="participant"></param>
		/// <returns></returns>
		public Task CreateTestParticipant(Participant participant);
	}
}
