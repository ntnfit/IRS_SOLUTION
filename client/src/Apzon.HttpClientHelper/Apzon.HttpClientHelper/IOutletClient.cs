using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IOutletClient
{
	Task<DataTable> GetList();
}
