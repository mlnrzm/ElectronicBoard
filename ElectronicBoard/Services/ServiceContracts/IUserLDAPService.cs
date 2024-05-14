using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IUserLDAPService
	{
		// Получение аккаунта по логину
		public Task<UserLDAP> GetElement(UserLDAP model);

		// Добавление аккаунта
		public Task Insert(UserLDAP model);

		// Удаление аккаунта
		public Task Delete(UserLDAP model);

		public void RemoveTable();
	}
}
