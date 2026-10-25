using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

internal class UserDefinedTextsClient : IUserDefinedTextsClient
{
	private const string FormController = "UserDefinedTexts";

	public async Task<DataTable> GetAll()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefinedTexts");
		return await http.GetAsync("GetAll");
	}

	public async Task<string> Remove(int absEntry)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefinedTexts");
		return await http.PostParamsAsJsonAsync("Remove", absEntry);
	}

	public async Task<string> Save(DataTable dataTable)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UserDefinedTexts");
		return await http.PostAsJsonAsync("Save", dataTable);
	}
}
