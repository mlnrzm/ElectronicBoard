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
	}
}
