using System.Web.Mvc;
using System.Web.Routing;

namespace Apzon.Api;

public class RouteConfig
{
	public static void RegisterRoutes(RouteCollection routes)
	{
		routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
		routes.MapRoute("Default", "api/{controller}/{action}/{id}", new
		{
			controller = "Home",
			action = "Index",
			id = UrlParameter.Optional
		});
	}
}
