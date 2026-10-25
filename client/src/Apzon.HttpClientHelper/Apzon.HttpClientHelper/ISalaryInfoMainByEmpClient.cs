using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISalaryInfoMainByEmpClient
{
	Task<DataTable> GetDetail(int empId);

	Task<string> SaveDoc(DataTable data);

	Task<string> DeleteDoc(int lineNum, int empId);
}
