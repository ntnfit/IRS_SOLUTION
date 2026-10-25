using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PlanningProduction;

public interface IMrpPlan
{
	DataSet RunMrpPlan(string bpCode, string timeType);

	HttpResult SaveMrpPlan(DataSet ds);

	int GetPage(int current, int type, string username, int branch);

	DataTable GetHeader(int id);

	DataSet GetDetail(int id);
}
