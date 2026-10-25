using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Dashboard;

public class DashboardCategoriesController : BaseApiController
{
	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.DashboardCategories.GetData();
	}

	[HttpGet]
	public string CheckExistDashboardTab(int CatId)
	{
		return UnitOfWork.DashboardCategories.CheckExistDashboardTab(CatId);
	}

	[HttpPost]
	public string SaveData(DataTable dtCategories)
	{
		return UnitOfWork.DashboardCategories.SaveData(dtCategories);
	}
}
