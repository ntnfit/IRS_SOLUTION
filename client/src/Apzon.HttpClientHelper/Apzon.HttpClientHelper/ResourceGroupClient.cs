using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public class ResourceGroupClient : IResourceGroupClient
{
	private const string Controller = "ResourceGroup";

	public async Task<DataTable> SearchData(string resGroupName)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ResourceGroup");
		return await http.GetAsync("SearchData", new { resGroupName });
	}

	public async Task<string> Save(DataTable dt)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "ResourceGroup");
		return await http.PostAsync("Save", dt);
	}
}
