using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Apzon.Api.Broker;
using Apzon.Commons;

namespace Apzon.Api.Attributes;

public class ValidateLoginSapExceptionHandling : ActionFilterAttribute
{
	public override void OnActionExecuting(HttpActionContext actionContext)
	{
		if (actionContext.Request.Headers.Contains("SessionId"))
		{
			string text = Function.ToString(actionContext.Request.Headers.GetValues("SessionId").FirstOrDefault());
			string text2 = "";
			if (!string.IsNullOrEmpty(text) && Global.SessionUserInfo.ContainsKey(text))
			{
				text2 = Global.SessionUserInfo[text].UserName;
			}
			if (Global.LoginSap && !text2.ToLower().Equals(Global.SuperUser))
			{
				if (string.IsNullOrEmpty(text))
				{
					HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
					{
						ReasonPhrase = "INVALIDSESSION",
						Content = new StringContent("INVALIDSESSION")
					};
					actionContext.Response = response;
					return;
				}
				if (BrokerService.GetUnitOfWork(text) == null)
				{
					HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.InternalServerError)
					{
						ReasonPhrase = "INVALIDSESSION",
						Content = new StringContent("INVALIDSESSION")
					};
					actionContext.Response = response2;
					return;
				}
				if (!Global.SessionUserInfo.ContainsKey(text) || string.IsNullOrEmpty(Global.SessionUserInfo[text].SessionDiServer))
				{
					HttpResponseMessage response3 = new HttpResponseMessage(HttpStatusCode.InternalServerError)
					{
						ReasonPhrase = "INVALID SESSION ID",
						Content = new StringContent("INVALID SESSION ID")
					};
					actionContext.Response = response3;
					return;
				}
			}
		}
		base.OnActionExecuting(actionContext);
	}
}
