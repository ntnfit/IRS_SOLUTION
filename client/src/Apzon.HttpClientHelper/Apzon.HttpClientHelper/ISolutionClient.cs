using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISolutionClient
{
	Task<DataTable> Search(int sltCode, string itemCode, int status);

	Task<DataTable> GetLine(int atcEntry);

	Task<DataTable> GetSolutionStatuses();

	Task<DataTable> GetOwner();

	Task<DataTable> FindDataNextLast(int id, int type);

	Task<string> Add(DataSet ds);

	Task<string> Update(DataSet ds);

	Task<string> DeleteDocs(int id);

	Task<int> GetMaxSltCode();
}
