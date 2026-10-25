using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class ResourceWorkScheduleClient : IResourceWorkScheduleClient
{
	public const string Controller = "ResourceWorkSchedule";

	public async Task<DataTable> SearchData(string code, string status, Enums.NavigateType type)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ResourceWorkSchedule");
		return await http.GetAsync("SearchData", new { code, status, type });
	}

	public async Task<DataTable> GetDataLine(string code, int year)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ResourceWorkSchedule");
		return await http.GetAsync("GetDataLine", new { code, year });
	}

	public async Task<HttpResult> Delete(string code)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ResourceWorkSchedule");
		return await http.PostAsync("Delete", code);
	}

	public async Task<HttpResult> AddNew(DataSet dtSave)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ResourceWorkSchedule");
		return await http.PostAsync("AddNew", dtSave);
	}

	public async Task<HttpResult> Update(DataSet dtSave)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ResourceWorkSchedule");
		return await http.PostAsync("Update", dtSave);
	}
}
