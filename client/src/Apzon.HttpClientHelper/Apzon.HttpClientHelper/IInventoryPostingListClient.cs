using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IInventoryPostingListClient
{
	Task<DataTable> GetData(DataTable dt);
}
