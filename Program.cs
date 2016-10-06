using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PowerGrid.API.Models;
using System.IO;

namespace PowerGrid.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.CaptureStartupErrors(true)
				.UseSetting("detailedErrors", "true")
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				 .UseIISIntegration()//
				.UseStartup<Program>()
				.Build();

			host.Run();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			services.AddSingleton<INoteRepository, NoteRepository>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			app.UseMvcWithDefaultRoute();
		}

	}
}