using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class ScheduleClient : IScheduleClient
{
	private const string Controller = "Schedule";

	public async Task<DataSet> GetDataLine(int id)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Schedule");
		return await http.GetAsync("GetDataLine", new { id });
	}

	public async Task<HttpResult> Save(DataSet ds)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Schedule");
		return await http.PostAsync("Save", ds);
	}

	public async Task<HttpResult> Update(DataSet ds)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Schedule");
		return await http.PostAsync("Update", ds);
	}

	public async Task<DataTable> GetDataDoc(int id, Enums.NavigateType type)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
		return await http.GetAsync("GetDataDoc", new { id, type });
	}

	public async Task<DataTable> GetDataMrp(int mrpCode)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
		return await http.GetAsync("GetDataMrp", new { mrpCode });
	}

	public async Task<DataTable> GetDataProductionOrder(string itemCode, string version, string sourceType, int sourceId, int sourceLine)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
		return await http.GetAsync("GetDataProductionOrder", new { itemCode, version, sourceType, sourceId, sourceLine });
	}

	public async Task<DataTable> GetDataWhs(string inputType, int inputId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
		return await http.GetAsync("GetWhs", new { inputType, inputId });
	}

	public HttpResult GetNewIdSchedule()
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Schedule");
		return httpClientHelper.Get("GetNewIdSchedule");
	}

	public DataTable GetdataBpItem(string bpCode, string itemCode, string shipDate)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
		return httpClientHelper.Get("GetdataBpItem", new { bpCode, itemCode, shipDate });
	}

	public async Task<DataTable> GetDocStructure()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
		return await http.GetAsync("GetDocStructure");
	}

	public async Task<string> CheckExistsDocInType(string code, string genType, string inputType)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Schedule");
		return await http.GetAsync("CheckExistsDocInType", new { code, genType, inputType });
	}
}
