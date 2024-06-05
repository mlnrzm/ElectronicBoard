using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using System.DirectoryServices.Protocols;
using System.Net;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для работы с подключением к LDAP
	/// </summary>
	public class ConnectionAccountsLDAP : IConnectionAccountsLDAP
	{
		private IConfigurationRoot MyConfig { get; set; }
		private string? ldapHost { get; set; }
		private int ldapPort { get; set; }
		private string? loginDN { get; set; }
		private string? password { get; set; }
		public ConnectionAccountsLDAP() 
		{
			MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			ldapHost = MyConfig.GetValue<string>("LDAPSettings:LdapHost");
			ldapPort = (int)MyConfig.GetValue<int>("LDAPSettings:LdapPort");
			loginDN = MyConfig.GetValue<string>("LDAPSettings:LdapLoginDN");
			password = MyConfig.GetValue<string>("LDAPSettings:LdapPassword");
		}

		/// <summary>
		/// Метод синхронизации Ldap
		/// </summary>
		/// <returns></returns>
		public async Task ConnectionLDAP(IUserLDAPService userLDAPService)
		{
			try
			{
				// Подключение к серверу LDAP
				var server = new LdapDirectoryIdentifier(ldapHost, ldapPort);

				// Креды для доступа к серверу
				var credentials = new NetworkCredential(loginDN, password);

				// Подключение к серверу LDAP
				var cn = new System.DirectoryServices.Protocols.LdapConnection(server);
				cn.SessionOptions.ProtocolVersion = 3;
				cn.AuthType = AuthType.Basic;
				cn.Bind(credentials);

				// Загрузка аккаунтов
				await DownloadUserAccounts(cn, userLDAPService);

				cn.Dispose();
			}
			catch (Exception ex)
			{
				throw new Exception($"Возникла ошибка в процессе синхронизации с LDAP { ex.Message }");
			}
		}

		/// <summary>
		/// Метод загрузки аккаунтов пользователей из LDAP
		/// </summary>
		/// <param name="cn"></param>
		/// <param name="userLDAPService"></param>
		/// <returns></returns>
		private async Task DownloadUserAccounts(System.DirectoryServices.Protocols.LdapConnection cn, IUserLDAPService userLDAPService)
		{
			string filter = "(&(objectClass=ulstuPerson)(accountStatus=active)(!(iduniv=SYSTEMACC)))";
			string[] attributes = { "cn", "uid", "userPassword" };
			string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";

			var req = new SearchRequest(searchBase, filter, SearchScope.Subtree, attributes);
			var resp = (SearchResponse)cn.SendRequest(req);

			foreach (SearchResultEntry entry in resp.Entries)
			{
				var user = new UserLDAP
				{
					UserFIO = GetStringAttribute(entry, "cn"),
					UserLogin = GetStringAttribute(entry, "uid"),
					UserPassword = GetStringAttribute(entry, "userPassword")
				};
				await userLDAPService.Insert(user);
			}
		}

		/// <summary>
		/// Метод для авторизации в LDAP
		/// </summary>
		/// <param name="user_login"></param>
		/// <param name="user_password"></param>
		/// <returns></returns>
		public bool CanAuthorize(string user_login, string user_password)
		{
			try
			{
				string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";

				// Подключение к серверу LDAP
				var server = new LdapDirectoryIdentifier(ldapHost, ldapPort);

				// Креды для доступа к серверу
				var credentials = new NetworkCredential($"uid={user_login},{searchBase}", user_password);

				// Создаем подключение к серверу LDAP
				var cn = new System.DirectoryServices.Protocols.LdapConnection(server);
				cn.SessionOptions.ProtocolVersion = 3;
				cn.AuthType = AuthType.Basic;
				cn.Bind(credentials);

				return true;
			}
			catch (Exception ex)
			{
				throw new Exception($"Возникла ошибка в процессе авторизации с LDAP {ex.Message}");
			}
		}

		private static string GetStringAttribute(SearchResultEntry entry, string key)
		{
			if (!entry.Attributes.Contains(key))
			{
				return string.Empty;
			}
			string[] rawVal = (string[])entry.Attributes[key].GetValues(typeof(string));
			return rawVal[0];
		}
	}
}
