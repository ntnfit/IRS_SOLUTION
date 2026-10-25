using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class WorkScheduleRegisterController : BaseApiController
{
	[HttpPost]
	public DataTable Search(DataTable dt)
	{
		return UnitOfWork.WorkScheduleRegister.Search(dt);
	}

	[HttpGet]
	public DataTable GetDetail(int empID)
	{
		return UnitOfWork.WorkScheduleRegister.GetDetail(empID);
	}

	[HttpGet]
	public string Remove(int lineNum, int empID)
	{
		return UnitOfWork.WorkScheduleRegister.Remove(lineNum, empID);
	}

	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		return UnitOfWork.WorkScheduleRegister.Save(dt);
	}
}
