using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources.Salary;

public class AllowanceTypeController : BaseApiController
{
	[HttpGet]
	public DataTable GetAllowanceType()
	{
		return UnitOfWork.AllowanceType.GetAllowanceType();
	}

	[HttpPost]
	public string RemoveAllowanceType([FromBody] int code)
	{
		return UnitOfWork.AllowanceType.RemoveAllowanceType(code);
	}

	[HttpPost]
	public HttpResult SaveAllowanceType([FromBody] DataTable dt)
	{
		return UnitOfWork.AllowanceType.SaveAllowanceType(dt);
	}

	[HttpGet]
	public DataTable GetPerIncomeTax()
	{
		return UnitOfWork.PerIncomeTax.GetPerIncomeTax();
	}

	[HttpGet]
	public DataTable GetDetail(int code)
	{
		return UnitOfWork.PerIncomeTax.GetDetail(code);
	}

	[HttpPost]
	public string RemovePerIncomeTax([FromBody] string code)
	{
		return UnitOfWork.PerIncomeTax.RemovePerIncomeTax(code);
	}

	[HttpPost]
	public HttpResult SavePersonalIncomeTax([FromBody] DataSet ds)
	{
		return UnitOfWork.PerIncomeTax.SavePersonalIncomeTax(ds, "A");
	}

	[HttpPost]
	public HttpResult UpdatePersonalIncomeTax([FromBody] DataSet ds)
	{
		return UnitOfWork.PerIncomeTax.SavePersonalIncomeTax(ds, "U");
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.PerIncomeTax.GetMaxCode();
	}

	[HttpGet]
	public DataTable FindDataNextLast_PerInTax(int id, int type)
	{
		return UnitOfWork.PerIncomeTax.FindDataNextLast_PerInTax(id, type);
	}
}
