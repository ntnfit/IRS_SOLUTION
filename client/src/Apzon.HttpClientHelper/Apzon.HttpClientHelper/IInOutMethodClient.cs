using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IInOutMethodClient
{
	Task<DataTable> GetAll();

	Task<string> Remove(int methodId);

	Task<string> Save(DataSet ds);
}
