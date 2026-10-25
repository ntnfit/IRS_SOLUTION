using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IExternalRoutingClient
{
	Task<DataTable> GetAllRoutings();

	Task<DataSet> GetLines(int absEntry);

	Task<string> SaveDoc(DataSet ds);

	Task<string> DeleteDoc(int absEntry);
}
