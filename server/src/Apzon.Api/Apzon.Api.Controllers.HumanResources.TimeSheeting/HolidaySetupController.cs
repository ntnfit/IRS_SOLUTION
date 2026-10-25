using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class HolidaySetupController : BaseApiController
{
	[HttpGet]
	public DataTable GetLine(string hldCode)
	{
		return UnitOfWork.HolidaySetup.GetLine(hldCode);
	}

	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.HolidaySetup.GetAll();
	}

	[HttpGet]
	public string Remove(string hldCode)
	{
		return UnitOfWork.HolidaySetup.Remove(hldCode);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.HolidaySetup.Save(ds);
	}
}
