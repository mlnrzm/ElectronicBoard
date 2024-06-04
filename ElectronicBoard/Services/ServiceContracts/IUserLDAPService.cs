using ElectronicBoard.Models;

namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для взаимодействия с сущностью "Пользователь"
	/// </summary>
	public interface IUserLDAPService
	{
		/// <summary>
		/// Метод для получения аккаунта по логину
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task<UserLDAP> GetElement(UserLDAP model);

		/// <summary>
		/// Метод для добавления аккаунта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Insert(UserLDAP model);

		/// <summary>
		/// Метод для удаления аккаунта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Task Delete(UserLDAP model);

		/// <summary>
		/// Метод для очистки таблицы с пользователями
		/// </summary>
		public void RemoveTable();
	}
}
