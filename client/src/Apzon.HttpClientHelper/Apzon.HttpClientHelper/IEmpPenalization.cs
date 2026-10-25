using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IEmpPenalization
{
	Task<DataTable> GetData(int docNo);

	Task<DataTable> GetPeriodRecords();

	Task<string> Remove(int docNo);

	Task<string> Save(DataSet ds);
}
