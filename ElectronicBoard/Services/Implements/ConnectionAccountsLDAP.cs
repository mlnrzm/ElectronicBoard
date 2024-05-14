using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Novell.Directory.Ldap;

namespace ElectronicBoard.Services.Implements
{
	public class ConnectionAccountsLDAP : IConnectionAccountsLDAP
	{
		public async Task ConnectionLDAP(IUserLDAPService userLDAPService)
		{
			string ldapHost = "lk.ustu";
			int ldapPort = 389;
			string loginDN = "cn=sciencemon,ou=services,dc=ams,dc=ulstu,dc=ru";
			string password = "oPma80K#mE5$h8Er71Ga";

			string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";
			string searchFilter = "objectClass=ulstuPerson";

			int ldapVersion = LdapConnection.LdapV3;
			try
			{
				LdapConnection conn = new LdapConnection();

				conn.Connect(ldapHost, ldapPort);
				conn.Bind(ldapVersion, loginDN, password);

				string[] requiredAttributes = { "uid", "cn", "userPassword" };
				ILdapSearchResults lsc = conn.Search(searchBase,
									LdapConnection.ScopeSub,
									searchFilter,
									requiredAttributes,
									false);

				while (lsc.HasMore())
				{
					LdapEntry nextEntry = null;
					try
					{
						nextEntry = lsc.Next();
					}
					catch (LdapException e)
					{
						throw new Exception(e.LdapErrorMessage);
					}

					// Атрибуты сущности
					LdapAttributeSet attributeSet = nextEntry.GetAttributeSet();
					System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

					string UserLogin = "";
					string UserPassword = "";
					string UserFIO = "";
					// Проход по атрибутам сущности
					while (ienum.MoveNext())
					{
						LdapAttribute attribute = (LdapAttribute)ienum.Current;

						// uid, cn, userPassword
						string attributeName = attribute.Name;
						string attributeVal = attribute.StringValue;

						if (attributeName.Contains("uid"))
						{
							UserLogin = attributeVal;
						}
						else if (attributeName.Contains("userPassword")) 
						{
							UserPassword = attributeVal;
						}
						else if (attributeName.Contains("cn"))
						{
							UserFIO = attributeVal;
						}
					}
					await userLDAPService.Insert(new UserLDAP { UserFIO = UserFIO, UserLogin = UserLogin, UserPassword = UserPassword });
				}
				conn.Disconnect();
			}
			catch (LdapException e)
			{
				throw new Exception(e.LdapErrorMessage);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}
