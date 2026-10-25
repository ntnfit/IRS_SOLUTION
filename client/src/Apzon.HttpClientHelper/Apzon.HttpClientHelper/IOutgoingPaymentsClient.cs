using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IOutgoingPaymentsClient
{
	Task<DataTable> GetLines(int docNo);

	Task<DataTable> GetDoc();

	Task<DataTable> Search(DataTable dtSearch);

	Task<string> Remove(int docNo);

	Task<string> Save(DataSet ds);
}
