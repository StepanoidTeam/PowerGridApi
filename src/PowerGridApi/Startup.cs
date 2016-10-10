using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PowerGridEngine;

namespace PowerGridApi
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();

			//init energo server
			EnergoServer.Current.Settings.SimpleOrGuidPlayerId = true;
			ServerContext.InitCurrentContext(EnergoServer.Current, new Logger());
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc();

			services.AddCors();

			var xmlPath = GetXmlCommentsPath();
			services.AddSwaggerGen();
			services.ConfigureSwaggerGen(options =>
			{
				options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info
				{
					Version = Controllers.CommonController.Version,
					Title = "Power Grid API",
					Description = "API for Power Grid Game developed by AgeStone Team",
					TermsOfService = "None"
				});
				options.IncludeXmlComments(xmlPath);
				options.DescribeAllEnumsAsStrings();
			});

			//todo: maybe this is redundant when we enable all origins below
			services.Configure<MvcOptions>(options =>
			{
				options.Filters.Add(new CorsAuthorizationFilterFactory("AllowSpecificOrigin"));

			});



		}

		private string GetXmlCommentsPath()
		{
			var app = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application;
			return System.IO.Path.Combine(app.ApplicationBasePath, "PowerGridApi.xml");
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();


			app.UseCors(builder =>
				builder.AllowAnyOrigin()
				.AllowAnyHeader()
			);


			app.UseMvc();
			app.UseSwagger();

			var ver = Controllers.CommonController.Version;
			app.UseSwaggerUi("api/help", string.Format("/swagger/{0}/swagger.json", ver));
		}
	}
}
