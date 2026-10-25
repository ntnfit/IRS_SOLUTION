using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IPluginManagerClient
{
	Task<DataTable> GetData(string databaseName);

	Task<string> Save(DataTable dt);

	Task<string> Delete(string plugName);
}
