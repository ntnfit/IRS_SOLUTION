using System.Data;

namespace Apzon.HttpClientHelper;

internal class IndustryClient : IIndustryClient
{
	private const string Controller = "Industry";

	public string DeleteDoc(int indCode)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Industry");
		return httpClientHelper.Post("DeleteDoc", indCode);
	}

	public bool ExistsIndCode(int indCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Industry");
		return httpClientHelper.GetBool("ExistsIndCode", new { indCode });
	}

	public DataTable LoadData()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Industry");
		return httpClientHelper.Get("LoadData");
	}

	public string SaveDoc(DataTable dt)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Industry");
		return httpClientHelper.Post("SaveDoc", dt);
	}
}
