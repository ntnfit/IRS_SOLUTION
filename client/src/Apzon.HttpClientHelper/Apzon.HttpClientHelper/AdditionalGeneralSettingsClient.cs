using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class AdditionalGeneralSettingsClient : IAdditionalGeneralSettingsClient
{
	private const string _controller = "AdditionalGeneralSettings";

	public async Task<DataSet> GetData()
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "AdditionalGeneralSettings");
		return await helper.GetAsync("GetData");
	}

	public async Task<HttpResult> Save(DataSet ds)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AdditionalGeneralSettings");
		return await helper.PostAsync("Save", ds);
	}

	public async Task<string> GetDefaultLng()
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "AdditionalGeneralSettings");
		return await helper.GetAsync("GetDefaultLng");
	}
}
