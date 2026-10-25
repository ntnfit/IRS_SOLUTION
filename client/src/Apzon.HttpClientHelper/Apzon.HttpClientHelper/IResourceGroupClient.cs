using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IResourceGroupClient
{
	Task<DataTable> SearchData(string resGroupName);

	Task<string> Save(DataTable dt);
}
