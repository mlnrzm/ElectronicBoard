using ElectronicBoard.Models;
using ElectronicBoard.Services.Implements;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IBoardService
	{
		// Получение всего списка досок
		public Task<List<Board>> GetFullList();

		// Получение списка досок по названию
		public Task<List<Board>> GetFilteredList(Board model);
		// Получение досок участника
		public Task<List<Board>> GetParticipantBoards(int participantId);

		// Получение доски по id или названию
		public Task<Board> GetElement(Board model);

		// Добавление доски
		public Task Insert(Board model);

		// Редактирование данных о доске
		public Task Update(Board model);

		// Удаление доски
		public Task Delete(Board model);

		// Создание общей доски
		public Task CreateMainBoard(Participant part);
	}
}
