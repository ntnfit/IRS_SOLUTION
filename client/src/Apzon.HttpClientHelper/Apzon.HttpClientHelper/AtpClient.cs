using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class AtpClient : IAtpClient
{
	private const string Controller = "Atp";

	public async Task<DataTable> LoadAtpType()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetDataAtpType");
	}

	public async Task<HttpResult> GetNewIdAtp()
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetNewIdAtp");
	}

	public async Task<DataTable> GetStructureInput()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetStructureInput");
	}

	public async Task<DataTable> GetDocStructure()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetDocStructure");
	}

	public async Task<DataTable> RunAtp(DataSet ds)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Atp");
		return await http.PostAsync("RunAtp", ds);
	}

	public async Task<HttpResult> Save(DataSet ds)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Atp");
		return await http.PostAsync("Save", ds);
	}

	public async Task<DataSet> SearchData(Enums.NavigateType type, int code)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("SearchData", new { type, code });
	}

	public async Task<HttpResult> AtpUpdate(DataSet ds)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Atp");
		return await http.PostAsync("AtpUpdate", ds);
	}

	public HttpResult GetNewIdScheduleForAtp()
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Atp");
		return httpClientHelper.Get("GetNewIdScheduleForAtp");
	}

	public async Task<DataSet> GetDataAtpForSchedle(int code)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetDataAtpForSchedle", new { code });
	}

	public async Task<DataSet> RunSchedule(string code, string type)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("RunSchedule", new { code, type });
	}

	public async Task<DataSet> GetDataLineSchedule(int id)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetDataLineSchedule", new { id });
	}

	public async Task<DataTable> GetDocScheduleStructure()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetDocScheduleStructure");
	}

	public async Task<HttpResult> SaveSchedule(DataSet dtSet)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Atp");
		return await http.PostAsync("SaveSchedule", dtSet);
	}

	public async Task<HttpResult> UpdateSchedule(DataSet dtSet)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Atp");
		return await http.PostAsync("UpdateSchedule", dtSet);
	}

	public async Task<DataTable> GetDataDocSchedule(int code, Enums.NavigateType type)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Atp");
		return await http.GetAsync("GetDataDocSchedule", new { code, type });
	}
}
