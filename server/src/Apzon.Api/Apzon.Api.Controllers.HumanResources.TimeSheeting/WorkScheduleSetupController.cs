using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class WorkScheduleSetupController : BaseApiController
{
	[HttpGet]
	public DataSet GetLine(int schId)
	{
		return UnitOfWork.WorkScheduleSetup.GetLine(schId);
	}

	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.WorkScheduleSetup.GetAll();
	}

	[HttpGet]
	public string Remove(int schId)
	{
		return UnitOfWork.WorkScheduleSetup.Remove(schId);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.WorkScheduleSetup.Save(ds);
	}
}
