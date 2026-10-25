using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;

namespace Apzon.Api;

public static class WebApiConfig
{
	public static void Register(HttpConfiguration config)
	{
		config.MessageHandlers.Add(new QueueRequestHandler());
		config.MapHttpAttributeRoutes();
		config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new
		{
			id = UrlParameter.Optional
		});
		config.Routes.MapHttpRoute("Approval", "api/{controller=ApprovalDecision}/{action}/{id}", new
		{
			id = UrlParameter.Optional
		});
		config.Routes.MapHttpRoute("QueryRoute", "api/q/{action=Table}/{category}/{name}/", new
		{
			controller = "Query"
		});
		config.Routes.MapHttpRoute("ContextActionHandle", "api/h/{action}/{name}/{method}/", new
		{
			controller = "ContextActionHandle"
		});
		MediaTypeHeaderValue item = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault((MediaTypeHeaderValue t) => t.MediaType == "application/xml");
		config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(item);
	}
}
