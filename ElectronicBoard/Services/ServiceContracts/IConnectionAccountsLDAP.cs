namespace ElectronicBoard.Services.ServiceContracts
{
	/// <summary>
	/// Интерфейс для работы с подключением к LDAP
	/// </summary>
	public interface IConnectionAccountsLDAP
	{
		/// <summary>
		/// Метод для загрузки списка учетных записей УлГТУ, хранящихся в LDAP
		/// </summary>
		/// <param name="userLDAPService"></param>
		/// <returns></returns>
		public Task ConnectionLDAP(IUserLDAPService userLDAPService);

		/// <summary>
		/// Метод для авторизации в LDAP
		/// </summary>
		/// <param name="user_login"></param>
		/// <param name="user_password"></param>
		/// <returns></returns>
		public bool CanAuthorize(string user_login, string user_password);
	}
}
