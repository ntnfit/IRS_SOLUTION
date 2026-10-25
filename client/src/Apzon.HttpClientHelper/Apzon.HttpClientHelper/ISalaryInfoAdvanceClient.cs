using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISalaryInfoAdvanceClient
{
	Task<DataTable> SearchByPeriod(DataTable dtSearch);

	Task<DataTable> GetAdvancedInfo(int empId);

	Task<string> SaveDoc(DataTable data);

	DataTable GetWorkPeriodsByBranch(int branch);

	Task<string> DeleteDoc(int lineNum, int empId);
}
