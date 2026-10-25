using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;

namespace Apzon.Api.Controllers.RoutingAndResource;

public class CapacityController : BaseApiController
{
	[HttpGet]
	public DataTable GetResources()
	{
		return UnitOfWork.Capacity.GetResources();
	}

	[HttpGet]
	public DataTable GetDataAppointments(int scheduleId, Enums.TransactionType transactionType)
	{
		return UnitOfWork.Capacity.GetDataAppointments(scheduleId, transactionType);
	}

	[HttpGet]
	public DataTable GetColorSetting()
	{
		return UnitOfWork.Capacity.GetColorSetting();
	}

	[HttpPost]
	public string Save(DataTable dt)
	{
		return UnitOfWork.Capacity.Save(dt);
	}

	[HttpGet]
	public DataTable GetDataResourceEffic(string resCode, string routCode, int routLine)
	{
		return UnitOfWork.Capacity.GetDataResourceEffic(resCode, routCode, routLine);
	}
}
