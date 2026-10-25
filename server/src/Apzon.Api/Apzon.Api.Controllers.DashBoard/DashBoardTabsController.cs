using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DashBoard;

public class DashBoardTabsController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllDashBoardTabs()
	{
		return UnitOfWork.DashBoardTabs.GetAllDashBoardTabs();
	}

	[HttpGet]
	public DataTable GetControllersByTab(int tabId)
	{
		return UnitOfWork.DashBoardTabs.GetControllersByTab(tabId);
	}

	[HttpGet]
	public DataTable GetAllDashBoardControllers()
	{
		return UnitOfWork.DashBoardTabs.GetAllDashBoardControllers();
	}

	[HttpGet]
	public DataTable GetAllTabsByCategory(int catId)
	{
		return UnitOfWork.DashBoardTabs.GetAllTabsByCategory(catId);
	}

	[HttpGet]
	public DataTable GetMaxControllerId()
	{
		return UnitOfWork.DashBoardTabs.GetMaxControllerId();
	}

	[HttpGet]
	public DataTable GetMaxTabId()
	{
		return UnitOfWork.DashBoardTabs.GetMaxTabId();
	}

	[HttpGet]
	public DataTable GetTabByTabId(int tabId)
	{
		return UnitOfWork.DashBoardTabs.GetTabByTabId(tabId);
	}

	[HttpGet]
	public DataTable GetTabByTabName(string tabName)
	{
		return UnitOfWork.DashBoardTabs.GetTabByTabName(tabName);
	}

	[HttpPost]
	public HttpResult SaveDoc([FromBody] DataSet ds)
	{
		return UnitOfWork.DashBoardTabs.SaveDoc(ds);
	}

	[HttpPost]
	public string DeleteTab([FromBody] int tabId)
	{
		return UnitOfWork.DashBoardTabs.DeleteTab(tabId);
	}
}
