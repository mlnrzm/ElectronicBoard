namespace ElectronicBoard.Services.ServiceContracts
{
	public interface IConnectionAccountsLDAP
	{
		public Task ConnectionLDAP(IUserLDAPService userLDAPService);
	}
}
