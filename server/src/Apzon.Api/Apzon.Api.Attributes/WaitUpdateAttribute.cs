using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Apzon.Api.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class WaitUpdateAttribute : ActionFilterAttribute
{
	public static List<string> LstDatabase = new List<string>();

	public override void OnActionExecuting(HttpActionContext actionContext)
	{
		bool flag = false;
		string text = "";
		if (actionContext.Request.Headers.Contains("Database"))
		{
			text = actionContext.Request.Headers.GetValues("Database").FirstOrDefault();
		}
		if (!string.IsNullOrEmpty(text))
		{
			lock (LstDatabase)
			{
				if (!LstDatabase.Contains(text))
				{
					flag = true;
					LstDatabase.Add(text);
				}
			}
			while (!flag)
			{
				Thread.Sleep(50);
				lock (LstDatabase)
				{
					if (!LstDatabase.Contains(text))
					{
						flag = true;
						LstDatabase.Add(text);
					}
				}
			}
		}
		base.OnActionExecuting(actionContext);
	}
}
