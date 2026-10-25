using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class SalaryInfoAdvanceController : BaseApiController
{
	[HttpPost]
	public DataTable SearchByPeriod([FromBody] DataTable dt)
	{
		return UnitOfWork.SalaryInfoAdvance.SearchByPeriod(dt);
	}

	[HttpGet]
	public DataTable GetAdvancedInfo(int empId)
	{
		return UnitOfWork.SalaryInfoAdvance.GetAdvancedInfo(empId);
	}

	[HttpGet]
	public DataTable GetWorkPeriodsByBranch(int branch)
	{
		return UnitOfWork.SalaryInfoAdvance.GetWorkPeriodsByBranch(branch);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.SalaryInfoAdvance.SaveDoc(dt);
	}

	[HttpGet]
	public string DeleteDoc(int lineNum, int empId)
	{
		return UnitOfWork.SalaryInfoAdvance.DeleteDoc(lineNum, empId);
	}
}
