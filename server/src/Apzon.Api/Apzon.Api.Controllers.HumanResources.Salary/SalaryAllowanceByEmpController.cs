using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class SalaryAllowanceByEmpController : BaseApiController
{
	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.SalaryAllowanceByEmp.SaveDoc(dt);
	}

	[HttpGet]
	public DataTable GetDetail(int empId)
	{
		return UnitOfWork.SalaryAllowanceByEmp.GetDetail(empId);
	}

	[HttpGet]
	public string DeleteDoc(int lineNum, int empId)
	{
		return UnitOfWork.SalaryAllowanceByEmp.DeleteDoc(lineNum, empId);
	}
}
