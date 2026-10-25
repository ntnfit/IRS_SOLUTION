using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface ILeaveInformation
{
	DataTable SearchLeaveInfo(DataTable dtSearch);

	HttpResult SaveLeaveInformation(DataTable dtSearch);

	DataTable GetWorkPeriodsByBranch(int branch);
}
