using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class CapacitySettingClient : ICapacitySettingClient
{
	private const string ControllerName = "CapacitySetting";

	public DataTable GetColumnSetting()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CapacitySetting");
		return httpClientHelper.Get("GetColumnSetting");
	}

	public DataSet GetDataSetting()
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CapacitySetting");
		return httpClientHelper.Get("GetDataSetting");
	}

	public HttpResult SaveData(DataSet dsSave)
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CapacitySetting");
		return httpClientHelper.Post("SaveData", dsSave);
	}
}
