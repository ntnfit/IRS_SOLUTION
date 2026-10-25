using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class DMSSettingUDFClient : IDMSSettingUDFClient
{
	private const string Controller = "DMSSettingUDF";

	public async Task<HttpResult> GetData(string tableName, int menuId)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "DMSSettingUDF");
		return await http.GetAsync("GetData", new { tableName, menuId });
	}

	public async Task<HttpResult> Update(DataTable dt)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "DMSSettingUDF");
		return await http.PostAsync("Update", dt);
	}
}
