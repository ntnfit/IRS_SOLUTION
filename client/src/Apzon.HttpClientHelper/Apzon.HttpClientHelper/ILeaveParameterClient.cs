using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ILeaveParameterClient
{
	Task<DataTable> GetDoc();

	Task<string> SaveDoc(DataTable data);

	Task<string> DeleteDoc(int code);
}
