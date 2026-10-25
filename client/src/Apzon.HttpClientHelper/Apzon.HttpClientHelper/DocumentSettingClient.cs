using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public class DocumentSettingClient : IDocumentSettingClient
{
	private const string ControllerName = "DocumentSettings";

	public async Task<DataSet> GetDataForm()
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DocumentSettings");
		return await helper.GetAsync("LoadDoc");
	}

	public async Task<string> Save(DataSet dsSave)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "DocumentSettings");
		return await helper.PostAsync("Save", dsSave);
	}

	public async Task<DataTable> GetDataSettingByObjectType(ApzObjectTypes objectType)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DocumentSettings");
		return await helper.GetAsync("GetDataSettingByObjectType", new { objectType });
	}
}
