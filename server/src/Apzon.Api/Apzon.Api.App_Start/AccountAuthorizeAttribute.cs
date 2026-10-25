using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Apzon.Api.Broker;
using Apzon.Commons;

namespace Apzon.Api.App_Start;

public class AccountAuthorizeAttribute : AuthorizeAttribute
{
	public override void OnAuthorization(HttpActionContext controllerContext)
	{
		try
		{
			string text = Function.ToString(controllerContext.Request.Headers.GetValues("SessionId").FirstOrDefault());
			if (!string.IsNullOrEmpty(text))
			{
				if (BrokerService.GetApzUnitOfWork(text) == null)
				{
					controllerContext.Response = new HttpResponseMessage();
				}
			}
			else
			{
				controllerContext.Response = new HttpResponseMessage();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			controllerContext.Response = new HttpResponseMessage();
		}
	}

	public bool IsRole()
	{
		return true;
	}
}
