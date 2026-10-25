using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class SalaryCalculationController : BaseApiController
{
	[HttpPost]
	public DataSet Calculalte(DataTable dt)
	{
		return UnitOfWork.SalaryCalculation.Calculalte(dt);
	}

	[HttpPost]
	public DataSet ViewSalary(DataTable dt)
	{
		return UnitOfWork.SalaryCalculation.ViewSalaryCalculation(dt);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataSet ds)
	{
		return UnitOfWork.SalaryCalculation.SaveDoc(ds);
	}

	[HttpPost]
	public DataTable SearchEmployees(DataTable dt)
	{
		return UnitOfWork.SalaryCalculation.SearchEmployees(dt);
	}

	[HttpPost]
	public DataTable GetSalarySlip(DataTable dt)
	{
		return UnitOfWork.SalaryCalculation.GetSalarySlip(dt);
	}

	[HttpPost]
	public string SaveSendMailStt([FromBody] DataSet ds)
	{
		return UnitOfWork.SalaryCalculation.SaveSendMailStt(ds);
	}

	[HttpPost]
	public string SaveRptFileName([FromBody] DataSet ds)
	{
		return UnitOfWork.SalaryCalculation.SaveRptFileName(ds);
	}

	[HttpGet]
	public DataTable GetRptFileName()
	{
		return UnitOfWork.SalaryCalculation.GetRptFileName();
	}
}
