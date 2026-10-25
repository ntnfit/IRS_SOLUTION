using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Dashboard;

public class DashboardDetailController : BaseApiController
{
	[HttpPost]
	public HttpResult AddDetailInForm([FromBody] DataSet ds)
	{
		return UnitOfWork.DashboardDetail.SaveDetailInForm(ds, "A");
	}

	[HttpPost]
	public HttpResult UpdateDetailInForm([FromBody] DataSet ds)
	{
		return UnitOfWork.DashboardDetail.SaveDetailInForm(ds, "U");
	}

	[HttpGet]
	public DataTable LoadAll()
	{
		return UnitOfWork.DashboardDetail.LoadAll();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dt)
	{
		return UnitOfWork.DashboardDetail.Save(dt);
	}

	[HttpPost]
	public HttpResult UpdateLayout([FromBody] Tuple<int, string> model)
	{
		return UnitOfWork.DashboardDetail.UpdateLayout(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable GetById(int id)
	{
		return UnitOfWork.DashboardDetail.GetById(id);
	}

	[HttpGet]
	public DataTable GetParams(int id)
	{
		return UnitOfWork.DashboardDetail.GetParams(id);
	}

	[HttpPost]
	public HttpResult SaveDataSet(DataSet ds)
	{
		return UnitOfWork.DashboardDetail.SaveDataSet(ds);
	}

	[HttpPost]
	public HttpResult Remove([FromBody] int dashBoardId)
	{
		return UnitOfWork.DashboardDetail.Remove(dashBoardId);
	}
}
