using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class LeaveInformationController : BaseApiController
{
	[HttpPost]
	public DataTable SearchLeaveInfo(DataTable dt)
	{
		return UnitOfWork.LeaveInformation.SearchLeaveInfo(dt);
	}

	[HttpPost]
	public HttpResult SaveLeaveInformation(DataTable dt)
	{
		return UnitOfWork.LeaveInformation.SaveLeaveInformation(dt);
	}

	[HttpGet]
	public DataTable GetWorkPeriodsByBranch(int branch)
	{
		return UnitOfWork.LeaveInformation.GetWorkPeriodsByBranch(branch);
	}
}
