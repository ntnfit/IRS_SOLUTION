using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class SalaryInfoAllowanceController : BaseApiController
{
	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.SalaryInfoAllowance.SaveDoc(dt);
	}

	[HttpPost]
	public DataTable SearchByDate(DataTable dt)
	{
		return UnitOfWork.SalaryInfoAllowance.SearchByDate(dt);
	}

	[HttpGet]
	public DataTable GetMaxLineNum(int empId)
	{
		return UnitOfWork.SalaryInfoAllowance.GetMaxLineNum(empId);
	}
}
