using System.Web.Mvc;

namespace Apzon.Api.Controllers;

public class HomeController : Controller
{
	public ActionResult Index()
	{
		return View();
	}
}
