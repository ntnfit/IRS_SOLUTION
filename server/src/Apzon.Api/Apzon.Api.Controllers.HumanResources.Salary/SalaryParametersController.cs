using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class SalaryParametersController : BaseApiController
{
	[HttpGet]
	public DataTable GetList()
	{
		return UnitOfWork.SalaryParameters.GetList();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
	{
		return UnitOfWork.SalaryParameters.Save(dt, "A");
	}

	[HttpPost]
	public string DeleteDocs([FromBody] string ids)
	{
		return UnitOfWork.SalaryParameters.DeleteDocs(ids);
	}

	[HttpGet]
	public DataTable GetByDate(DateTime fromDate, DateTime toDate)
	{
		return UnitOfWork.SalaryParameters.GetByDate(fromDate, toDate);
	}
}
