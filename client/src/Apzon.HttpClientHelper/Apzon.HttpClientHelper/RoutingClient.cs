using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class RoutingClient : IRoutingClient
{
	private const string Controller = "AdvanceRouting";

	public async Task<DataTable> GetDateDoc(string routCode, string status, string fDate, string tDate, Enums.NavigateType transType)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AdvanceRouting");
		return await http.GetAsync("GetDataDoc", new { routCode, status, fDate, tDate, transType });
	}

	public async Task<DataTable> GetDataLine(string routCode)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AdvanceRouting");
		return await http.GetAsync("GetDataLine", new { routCode });
	}

	public async Task<HttpResult> Save(DataSet ds, Enums.TransactionType transaction)
	{
		string action = ((transaction == Enums.TransactionType.A) ? "Save" : "Update");
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AdvanceRouting");
		return await http.PostAsync(action, ds);
	}
}
