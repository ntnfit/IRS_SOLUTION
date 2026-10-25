using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class CompanyDetailsController : BaseApiController
{
	[HttpGet]
	public DataTable GetOADM()
	{
		return UnitOfWork.CompanyDetails.GetOADM();
	}

	[HttpGet]
	public DataTable GetADM1()
	{
		return UnitOfWork.CompanyDetails.GetADM1();
	}

	[HttpGet]
	public DataTable GetTableColumns(string tableName)
	{
		return UnitOfWork.CompanyDetails.GetTableColumns(tableName);
	}

	[HttpPost]
	public DataTable GetListState(string country)
	{
		return new DataTable();
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "This function is not supported";
		}
		return UnitOfWork.CompanyDetails.Save(ds);
	}

	[HttpGet]
	public DataTable GetHoliday()
	{
		return UnitOfWork.CompanyDetails.GetHoliday();
	}

	[HttpGet]
	public DataTable GetCountry()
	{
		return UnitOfWork.CompanyDetails.GetCountry();
	}

	[HttpGet]
	public DataTable GetState(string codeCountry)
	{
		return UnitOfWork.CompanyDetails.GetState(codeCountry);
	}
}
