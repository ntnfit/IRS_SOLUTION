using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISalaryInfoClient
{
	DataTable GetMaxLineNum(int empId);

	Task<string> SaveDoc(DataTable data);

	Task<DataSet> Search(DataTable dt);

	Task<DataTable> SearchByDate(DataTable dt);
}
