using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISalaryInfoAllowanceClient
{
	Task<string> SaveDoc(DataTable data);

	Task<DataTable> SearchByDate(DataTable dt);

	DataTable GetMaxLineNum(int empId);
}
