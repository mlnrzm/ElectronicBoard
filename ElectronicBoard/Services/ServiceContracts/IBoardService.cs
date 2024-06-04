using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Доска"
	/// </summary>
	public interface IBoardService
	{
		/// <summary>
		/// Метод для получения списка досок
		/// </summary>
		/// <returns></returns>
		public Task<List<Board>> GetFullList();

		/// <summary>
		/// Метод для получения списка досок участника по Id участника
		/// </summary>
		/// <param name="participantId"></param>
		/// <returns></returns>
		public Task<List<Board>> GetParticipantBoards(int participantId);

		/// <summary>
		/// Метод для получения доски по Id или названию
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<Board?> GetElement(Board model);

		/// <summary>
		/// Метод для добавления доски
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(Board model);

		/// <summary>
		/// Метод для редактирования доски
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Update(Board model);

		/// <summary>
		/// Метод для удаления доски
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(Board model);

		/// <summary>
		/// Метод для создания общей доски
		/// </summary>
		/// <param name="part"></param>
		/// <returns></returns>
		public Task CreateMainBoard(Participant part);
	}
}
