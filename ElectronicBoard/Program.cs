using ElectronicBoard.Models;
using System.IO;
using System.Text;

namespace ElectronicBoard
{
	public class Program
	{
		// Время последнего обновления учётных записей
		public static DateTime? DateUpdate { get; set; }
		// Общая доска научных групп
		public static Board MainBoard { get; set; }

		public static void Main(string[] args)
		{

			string rootPath = @"C:\Users\User\source\repos\ElectronicBoard";
			var header = "***********************************" + Environment.NewLine;

			var files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

			var result = files.Select(path => new { Name = Path.GetFileName(path), Contents = System.IO.File.ReadAllText(path) })
							  .Select(info =>
								  header
								+ "Filename: " + info.Name + Environment.NewLine
								+ header
								+ info.Contents);


			var singleStr = string.Join(Environment.NewLine, result);
			Console.WriteLine(singleStr);
            System.IO.File.WriteAllText(@"C:\tmp\output.txt", singleStr, Encoding.UTF8);

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
