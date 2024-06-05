using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
	/// <summary>
	/// Класс для взаимодействия с сущностью "Пользователь"
	/// </summary>
	public class UserLDAPService : IUserLDAPService
	{
		private IConnectionAccountsLDAP con { get; set; }
		public UserLDAPService(IConnectionAccountsLDAP _conn) { con = _conn; }

		public UserLDAPService() {}

		/// <summary>
		/// Метод для получения аккаунта по логину
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task<UserLDAP?> GetElement(UserLDAP model)
		{
			await UpdateAccounts();
			if (model == null)
			{
				return null;
			}
			using var context = new ElectronicBoardDatabase();
			var component = await context.UserLDAPs
			.FirstOrDefaultAsync(rec => rec.UserLogin.Contains(model.UserLogin));
			return component != null ? CreateModel(component) : null;
		}

		/// <summary>
		/// Метод для добавления аккаунта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Insert(UserLDAP model)
		{
			using var context = new ElectronicBoardDatabase();
			await context.UserLDAPs.AddAsync(CreateModel(model, new UserLDAP()));
			await context.SaveChangesAsync();
		}

		/// <summary>
		/// Метод для удаления аккаунта
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public async Task Delete(UserLDAP model)
		{
			using var context = new ElectronicBoardDatabase();
			context.UserLDAPs.Remove(model);
			await context.SaveChangesAsync();
		}

		/// <summary>
		/// Проверка правильности пароля
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public bool CheckPassword(UserLDAP model) 
		{
			return con.CanAuthorize(model.UserLogin, model.UserPassword);
		}

		/// <summary>
		/// Метод для очистки таблицы с пользователями
		/// </summary>
		public void RemoveTable()
		{
			using (var context = new ElectronicBoardDatabase())
			{
				context.UserLDAPs.RemoveRange(context.UserLDAPs);
				context.SaveChanges();
			}
		}

		/// <summary>
		/// Метод для обновления списка аккаунтов
		/// </summary>
		private async Task UpdateAccounts() 
		{
			double clock = 0;
			if (Program.DateUpdate != null)
			{
				TimeSpan time = (TimeSpan)(DateTime.Now - Program.DateUpdate);
				clock = time.TotalHours;
			}
			if (clock > 8 || Program.DateUpdate == null)
			{
				RemoveTable();
				await con.ConnectionLDAP(this);
				Program.DateUpdate = DateTime.Now;
			}
		}
		public UserLDAP CreateModel(UserLDAP model, UserLDAP user)
		{
			user.UserFIO = model.UserFIO;
			user.UserLogin = model.UserLogin;
			user.UserPassword = model.UserPassword;

			return user;
		}
		private static UserLDAP CreateModel(UserLDAP user)
		{
			return new UserLDAP
			{
				Id = user.Id,
				UserFIO = user.UserFIO,
				UserLogin = user.UserLogin,
				UserPassword = user.UserPassword
			};
		}
	}
}
