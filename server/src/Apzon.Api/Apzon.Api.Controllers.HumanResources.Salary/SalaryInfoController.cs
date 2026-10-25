using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class SalaryInfoController : BaseApiController
{
	[HttpPost]
	public DataSet Search(DataTable dt)
	{
		return UnitOfWork.SalaryInfo.Search(dt);
	}

	[HttpPost]
	public DataTable SearchByDate(DataTable dt)
	{
		return UnitOfWork.SalaryInfo.SearchByDate(dt);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		return UnitOfWork.SalaryInfo.SaveDoc(dt);
	}

	[HttpGet]
	public DataTable GetMaxLineNum(int empId)
	{
		return UnitOfWork.SalaryInfo.GetMaxLineNum(empId);
	}
}
