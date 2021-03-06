﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PowerGridEngine;
using System.Globalization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.IO;
using System.Text;

//[assembly: AssemblyVersion("0.0.*")]

namespace PowerGridApi.Controllers
{
	/// <summary>
	/// Base controller class for any controller in API
	/// </summary>
	[EnableCors("CorsPolicy")]
	public abstract partial class BaseController : Controller
    {
        private static string _logFilePath = "Log_{0}.txt";

		private static bool _enableLogging = false;

		private static decimal _version = 0.01m;

        protected const string BroadcastReason = "BroadcastReason";

        public static string SwaggerVersion
		{
			get
			{
				return string.Format("v{0}", _version.ToString(CultureInfo.InvariantCulture));
			}
		}

		/// <summary>
		/// Version of current API
		/// </summary>
		public static Tuple<string, string> Version
        {
            get
            {
                var vers = System.Reflection.Assembly.GetEntryAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();
                //version has such format: <VersionMajor.VersionMinor.MonthDay.HoursMinutes> (0.0.327.1851)
                var parts = vers.Split('.');
                var year = DateTime.UtcNow.Year;
                var monthAndDay = int.Parse(parts[2]);
                var month = monthAndDay / 100;
                var day = monthAndDay - month * 100;
                var hoursAndMins = int.Parse(parts[3]);
                var hours = hoursAndMins / 100;
                var mins = hoursAndMins - hours * 100;
                var updatedUtcTime = new DateTime(year, month, day, hours, mins, 0, DateTimeKind.Utc);
                var timeZone = "FLE Standard Time";
                var updatetTime = TimeZoneInfo.ConvertTime(updatedUtcTime, TimeZoneInfo.FindSystemTimeZoneById(timeZone));

                return new Tuple<string, string>(vers, string.Format("{0}({1})", updatetTime.ToString("yyyy-MM-dd HH:mm"), timeZone));
            }
        }


		protected BaseController()
		{
		}

		protected string RawRequest { get; set; }

		public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			//todo bad code, need to investigate how to collect logs in correct way
			foreach (var model in context.ActionArguments)
				RawRequest = string.Format("{0} = {1}{2}", model.Key, model.Value.ToJson(), Environment.NewLine);

			if (await IfAuthorized(context, next))
			{
				if (await IfNotRestricted(context, next))
					await base.OnActionExecutionAsync(context, next);
			}
		}

		public override async void OnActionExecuted(ActionExecutedContext context)
		{
			if (!_enableLogging)
				return;

			var sb = new StringBuilder();
			var inputStream = context.HttpContext.Request.Body;
			var curDt = DateTime.UtcNow;
			sb.AppendLine(string.Format("Request at {0}:", curDt.ToString("yyyy-MM-ddTHH:mm:ss")));
			sb.AppendLine(string.Format("Path: {0} {1}", Request.Method, Request.Path));
			//sb.AppendLine(string.Format("User: {0}", UserContext == null ? "<none>" : UserContext.User.AuthToken));
			if (context.Exception != null)
			{
				sb.AppendLine(string.Format("Exception: {0}", context.Exception.Message));
				sb.AppendLine(string.Format("Stack trace: {0}", context.Exception.StackTrace));
			}
			else
				sb.AppendLine(string.Format("Body: {0}", RawRequest));
			sb.AppendLine(string.Format("Response: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(context.Result)));
			sb.AppendLine("-----------------------------------------------");
			var path = string.Format(_logFilePath, curDt.ToString("yyyy-MM-dd"));
			using (var file = new StreamWriter(System.IO.File.Open(path, FileMode.Append)))
				await file.WriteAsync(sb.ToString());
		}
	}
}
