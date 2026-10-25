using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class TaxGroupsClient : ITaxGroupsClient
{
	private const string FormController = "VatGroups";

	public string CheckCode(string code)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "VatGroups");
		return httpClientHelper.Get("CheckCode", new { code });
	}

	public string Delete(string code)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "VatGroups");
		return httpClientHelper.Post("Delete", code);
	}

	public bool Exists(string code)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "VatGroups");
		return httpClientHelper.GetBool("Exists", new { code });
	}

	public async Task<DataTable> GetAllLine_VTG1()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
		return await hp.GetAsync("GetAllLine_VTG1");
	}

	public async Task<DataTable> GetStructHeader()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
		return await hp.GetAsync("GetStructHeader");
	}

	public async Task<DataTable> GetStructLines()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
		return await hp.GetAsync("GetStructHeader");
	}

	public async Task<DataTable> GetDataOVTG(string vatType)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "VatGroups");
		return await hp.GetAsync("GetDataOVTG", new { vatType });
	}

	public HttpResult SaveVat(DataSet ds)
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "VatGroups");
		return httpClientHelper.Post("SaveVat", ds);
	}
}
