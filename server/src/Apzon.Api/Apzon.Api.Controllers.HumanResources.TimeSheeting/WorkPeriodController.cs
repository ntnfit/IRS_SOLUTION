using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class WorkPeriodController : BaseApiController
{
	[HttpPost]
	public HttpResult Add([FromBody] DataTable dt)
	{
		return UnitOfWork.WorkPeriod.Save(dt, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataTable dt)
	{
		return UnitOfWork.WorkPeriod.Save(dt, "U");
	}

	[HttpPost]
	public string Remove([FromBody] Tuple<string, int> model)
	{
		return UnitOfWork.WorkPeriod.Remove(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable FindDataNextLast(string id, int type)
	{
		return UnitOfWork.WorkPeriod.FindDataNextLast(id, type);
	}

	[HttpGet]
	public DataTable GetListWorkPeriod()
	{
		return UnitOfWork.WorkPeriod.GetListWorkPeriod();
	}

	[HttpGet]
	public DataTable GetWorkPeriodByCode(string code, int branch)
	{
		return UnitOfWork.WorkPeriod.GetWorkPeriodByCode(code, branch);
	}

	[HttpGet]
	public DataTable GetByDate(DateTime fromDate, DateTime toDate, string code)
	{
		return UnitOfWork.WorkPeriod.GetByDate(fromDate, toDate, code);
	}
}
