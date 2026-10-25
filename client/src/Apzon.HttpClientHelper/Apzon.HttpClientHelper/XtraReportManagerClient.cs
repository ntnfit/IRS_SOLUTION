using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

internal class XtraReportManagerClient : IXtraReportManagerClient
{
	private const string Controller = "XtraReportManager";

	public async Task<string> GetDataReportLayoutById(int id)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "XtraReportManager");
		return await http.GetAsync("GetDataReportLayoutById", new { id });
	}
}
