using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Apzon.Commons.Enums;

namespace Apzon.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class MappingSapAttribute : ActionFilterAttribute
{
	public bool IsMapping { get; set; }

	public override void OnActionExecuting(HttpActionContext actionContext)
	{
		if (!IsMapping && Global.LoginSap && ((Global.DatabaseSystemType == Enums.DatabaseSystemType.SQL && actionContext.Request.Method == HttpMethod.Post) || actionContext.Request.Method == HttpMethod.Put || actionContext.Request.Method == HttpMethod.Delete))
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
			{
				ReasonPhrase = "This function is not supported"
			};
			actionContext.Response = response;
		}
		else
		{
			base.OnActionExecuting(actionContext);
		}
	}
}
