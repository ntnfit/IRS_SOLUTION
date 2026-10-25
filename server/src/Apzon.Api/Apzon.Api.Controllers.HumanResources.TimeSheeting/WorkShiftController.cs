using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class WorkShiftController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.WorkShift.GetAll();
	}

	[HttpGet]
	public DataTable GetLine(int shiftID)
	{
		return UnitOfWork.WorkShift.GetLine(shiftID);
	}

	[HttpPost]
	public string Remove([FromBody] int shiftId)
	{
		return UnitOfWork.WorkShift.Remove(shiftId);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.WorkShift.Save(ds);
	}
}
