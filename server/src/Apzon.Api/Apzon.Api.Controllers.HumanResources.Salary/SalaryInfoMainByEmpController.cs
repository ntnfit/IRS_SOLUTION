using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class SalaryInfoMainByEmpController : BaseApiController
{
	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.SalaryInfoMainByEmp.SaveDoc(dt);
	}

	[HttpGet]
	public DataTable GetDetail(int empId)
	{
		return UnitOfWork.SalaryInfoMainByEmp.GetDetail(empId);
	}

	[HttpGet]
	public string DeleteDoc(int lineNum, int empId)
	{
		return UnitOfWork.SalaryInfoMainByEmp.DeleteDoc(lineNum, empId);
	}
}
