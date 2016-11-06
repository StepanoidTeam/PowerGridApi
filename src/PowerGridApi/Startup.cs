﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PowerGridEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerGridApi
{
    //public class SwaggerParameter : Swashbuckle.Swagger.Model.IParameter
    //{
    //    public string Description { get; set; }
    //    public string In { get; set; }
    //    public string Name { get; set; }
    //    public bool Required { get; set; }

    //    public Dictionary<string, object> Extensions { get { return new Dictionary<string, object>(); } }
    //}

    //public class AddCustomAuthHeaderParameterOperationFilter : Swashbuckle.SwaggerGen.Generator.IOperationFilter
    //{
    //    public void Apply(Swashbuckle.Swagger.Model.Operation operation, Swashbuckle.SwaggerGen.Generator.OperationFilterContext context)
    //    {
    //        var param = new SwaggerParameter
    //        {
    //            Name = "CustomAuth",
    //            In = "header",
    //            Description = "access token",
    //            Required = false,
    //        };
    //        if (operation.Parameters != null && operation.OperationId == "ApiLogoutByUserIdPost")
    //            operation.Parameters.Add(param);
    //    }
    //}

    public partial class Startup
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
			EnergoServer.Current.Settings.SimpleOrGuidPlayerId = false;
			ServerContext.InitCurrentContext(EnergoServer.Current, new Logger());
        }

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
            // Add service and create Policy with options
            services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
			});

			//services.AddCors();


			// Add framework services.
			services.AddMvc();


            var responseStatusesLst = (IEnumerable<ResponseType>)Enum.GetValues(typeof(ResponseType));
            var responseStatuses = string.Join(", ", responseStatusesLst.Select(m => m.ToString()));

            //Configure Swagger - tool for UI Help about API
            var xmlPath = GetXmlCommentsPath();
			services.AddSwaggerGen();
			services.ConfigureSwaggerGen(options =>
			{
                options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info
                {
                    Version = Controllers.CommonController.Version,
                    Title = "Power Grid API",
                    Description = string.Format("API for Power Grid Game developed by AgeStone Team.<br/> All Possible response statuses: {0}", responseStatuses),
                    TermsOfService = "None"
				});
				options.IncludeXmlComments(xmlPath);
				options.DescribeAllEnumsAsStrings();
                //options.OperationFilter<AddCustomAuthHeaderParameterOperationFilter>();
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
            app.UseStaticFiles();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();


            //Configure CORS
			app.UseCors("CorsPolicy");
			//app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());


			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Common}/{action=GetVersion}/{id?}");
			});


            //Configure Authorization
            //ConfigureOAuth(app);

            //Configure Swagger - tool for UI Help about API
            app.UseSwagger();
            app.UseSwaggerUi("api/help", string.Format("/swagger/{0}/swagger.json", Controllers.CommonController.Version));
            

        }
	}
}
