using ElectronicBoard.Models;
using System.Security.Claims; 
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace ElectronicBoard
{
	public class Program
	{
		// �������������� ������������ � �������
		public static Participant Participant { get; set; }

		// �����  ��������������� ������������
		public static List<Board> Boards { get; set; }

		// ����� ����� ������� �����
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
