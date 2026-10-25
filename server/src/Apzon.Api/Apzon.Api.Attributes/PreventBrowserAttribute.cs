using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Apzon.Commons;

namespace Apzon.Api.Attributes;

public class PreventBrowserAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(HttpActionContext actionContext)
	{
		string userAgent = HttpContext.Current.Request.UserAgent;
		HttpBrowserCapabilities httpBrowserCapabilities = new HttpBrowserCapabilities
		{
			Capabilities = new Hashtable { 
			{
				string.Empty,
				userAgent
			} }
		};
		new BrowserCapabilitiesFactory().ConfigureBrowserCapabilities(new NameValueCollection(), httpBrowserCapabilities);
		if (!Function.ToString(httpBrowserCapabilities.Browser).ToUpper().Equals("UNKNOWN"))
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
			{
				ReasonPhrase = "INVALIDREQUEST"
			};
			actionContext.Response = response;
		}
		else
		{
			base.OnActionExecuting(actionContext);
		}
	}
}
