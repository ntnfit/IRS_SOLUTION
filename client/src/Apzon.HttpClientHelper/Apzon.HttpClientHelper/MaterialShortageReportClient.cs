using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.HttpClientHelper;

internal class MaterialShortageReportClient : IMaterialShortageReportClient
{
	private const string Controller = "MaterialShortageReport";

	public async Task<DataTable> Run(MaterialShortageReport_Impl input)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialShortageReport");
		return await http.PostAsync("Run", input);
	}

	public async Task<DataSet> SearchData(Enums.NavigateType type, int code)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "MaterialShortageReport");
		return await http.GetAsync("SearchData", new { type, code });
	}

	public async Task<HttpResult> GetNewId()
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "MaterialShortageReport");
		return await http.GetAsync("GetNewId");
	}

	public async Task<DataTable> LoadAtpType()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialShortageReport");
		return await http.GetAsync("LoadAtpType");
	}

	public async Task<DataTable> GetDocStructure()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialShortageReport");
		return await http.GetAsync("GetDocStructure");
	}

	public async Task<HttpResult> AddNew(DataSet ds)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "MaterialShortageReport");
		return await http.PostAsync("AddNew", ds);
	}

	public async Task<HttpResult> Update(DataSet ds)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "MaterialShortageReport");
		return await http.PostAsync("Update", ds);
	}
}
