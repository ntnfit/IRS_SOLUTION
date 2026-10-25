using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Dashboard;

public class CategoriesDABController : BaseApiController
{
	[HttpGet]
	public DataTable GetDataCategories()
	{
		return UnitOfWork.CategoriesDAB.GetDataCategories();
	}

	[HttpPost]
	public HttpResult UpdateCaption(DataTable dt)
	{
		return UnitOfWork.CategoriesDAB.UpdateCaption(dt);
	}

	[HttpGet]
	public DataTable GetTreeListCategory(string type, int userId)
	{
		return UnitOfWork.CategoriesDAB.GetTreeListCategory(type, userId);
	}

	[HttpGet]
	public DataSet GetDetailByDasId(int dasId, int userId)
	{
		return UnitOfWork.CategoriesDAB.GetDetailByDasId(dasId, userId);
	}

	[HttpGet]
	public DataTable GetLayoutByCatId(int userId)
	{
		return UnitOfWork.CategoriesDAB.GetLayoutByCatId(userId);
	}

	[HttpGet]
	public DataTable GetXMLByDasId(int DashId, int userId)
	{
		return UnitOfWork.CategoriesDAB.GetXMLByDasId(DashId, userId);
	}

	[HttpPost]
	public string SaveDashboard([FromBody] Tuple<string, string, int, int> model)
	{
		return UnitOfWork.CategoriesDAB.SaveDashboard(model.Item1, model.Item2, model.Item3, model.Item4);
	}

	[HttpPost]
	public HttpResult UpdateDashboard([FromBody] Tuple<string, int, int> model)
	{
		return UnitOfWork.CategoriesDAB.UpdateDashboard(model.Item1, model.Item2, model.Item3);
	}

	[HttpGet]
	public DataTable LoadData(int DashId, int userId)
	{
		return UnitOfWork.CategoriesDAB.LoadData(DashId, userId);
	}

	[HttpGet]
	public DataTable GetParameterCatId(int catId, int userId)
	{
		return UnitOfWork.CategoriesDAB.GetParameterCatId(catId, userId);
	}

	[HttpGet]
	public DataTable GetLookupCategories()
	{
		return UnitOfWork.CategoriesDAB.GetLookupCategories();
	}

	[HttpPost]
	public string Remove([FromBody] Tuple<int, int> model)
	{
		return UnitOfWork.CategoriesDAB.Remove(model.Item1, model.Item2);
	}

	[HttpPost]
	public string DeleteCategory([FromBody] string ids)
	{
		return UnitOfWork.CategoriesDAB.DeleteCategory(ids);
	}

	[HttpPost]
	public HttpResult SaveCategories([FromBody] DataTable dt)
	{
		return UnitOfWork.CategoriesDAB.SaveCategories(dt);
	}

	[HttpPost]
	public HttpResult RemoveCategory([FromBody] int id)
	{
		return UnitOfWork.CategoriesDAB.RemoveCategory(id);
	}
}
