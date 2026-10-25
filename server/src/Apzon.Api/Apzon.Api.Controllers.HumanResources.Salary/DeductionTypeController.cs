using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class DeductionTypeController : BaseApiController
{
	[HttpGet]
	public DataTable FindDataNextLast(int id, int type)
	{
		return UnitOfWork.DeductionType.FindDataNextLast(id, type);
	}

	[HttpGet]
	public DataTable GetList()
	{
		return UnitOfWork.DeductionType.GetList();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataSet ds)
	{
		return UnitOfWork.DeductionType.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.DeductionType.Save(ds, "U");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.DeductionType.DeleteDocs(ids);
	}

	[HttpGet]
	public DataTable GetDetail(int code)
	{
		return UnitOfWork.DeductionType.GetDetail(code);
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.DeductionType.GetMaxCode();
	}

	[HttpGet]
	public DataTable GetByDate(DateTime fromDate, DateTime toDate)
	{
		return UnitOfWork.DeductionType.GetByDate(fromDate, toDate);
	}
}
