using ElectronicBoard.Models;
using System.Security.Claims; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace ElectronicBoard
{
	public class Program
	{
		// Авторизованный пользователь в системе
		public static Participant Participant { get; set; }

		// Доски  авторизованного пользователя
		public static List<Board> Boards { get; set; }

		// Общая доска научных групп
		public static Board MainBoard { get; set; }

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
