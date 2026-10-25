using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public class PluginManagerClient : IPluginManagerClient
{
	private const string Controller = "PluginManager";

	public async Task<DataTable> GetData(string databaseName)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PluginManager");
		return await http.GetAsync("GetData", new { databaseName });
	}

	public async Task<string> Save(DataTable dt)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "PluginManager");
		return await http.PostAsync("Save", dt);
	}

	public async Task<string> Delete(string plugName)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "PluginManager");
		return await http.PostAsync("Delete", plugName);
	}
}
