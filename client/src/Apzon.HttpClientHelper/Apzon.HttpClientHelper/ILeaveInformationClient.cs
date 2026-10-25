using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ILeaveInformationClient
{
	Task<DataTable> SearchLeaveInfo(DataTable dt);

	Task<HttpResult> SaveLeaveInformation(DataTable dt);

	DataTable GetWorkPeriodsByBranch(int branch);
}
