using System.Drawing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Colorful.Console;

namespace ApiServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// 웰컴 배너
			DrawBanner();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		/// <summary>
		/// 웰컴 배너
		/// </summary>
		private static void DrawBanner()
		{
			WriteAscii("ROOT SEARCH SERVER", Color.DeepSkyBlue);
		}
	}
}
