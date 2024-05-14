using ElectronicBoard.Models;
using ElectronicBoard.Services.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace ElectronicBoard.Services.Implements
{
	public class UserLDAPService : IUserLDAPService
	{
		private IConnectionAccountsLDAP con { get; set; }
		public UserLDAPService(IConnectionAccountsLDAP _conn) { con = _conn; }
		public async Task<UserLDAP> GetElement(UserLDAP model)
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
		public async Task Insert(UserLDAP model)
		{
			using var context = new ElectronicBoardDatabase();
			await context.UserLDAPs.AddAsync(CreateModel(model, new UserLDAP()));
			await context.SaveChangesAsync();
		}
		public async Task Delete(UserLDAP model)
		{
			using var context = new ElectronicBoardDatabase();
			context.UserLDAPs.Remove(model);
			await context.SaveChangesAsync();
		}
		private static UserLDAP CreateModel(UserLDAP model, UserLDAP user)
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
		public void RemoveTable()
		{
			using (var context = new ElectronicBoardDatabase())
			{
				context.UserLDAPs.RemoveRange(context.UserLDAPs);
				context.SaveChanges();
			}
		}
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
	}
}
