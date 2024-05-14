using ElectronicBoard.Models;

namespace ElectronicBoard
{
	public class Program
	{
		// ����� ���������� ���������� ������� �������
		public static DateTime? DateUpdate { get; set; }
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
