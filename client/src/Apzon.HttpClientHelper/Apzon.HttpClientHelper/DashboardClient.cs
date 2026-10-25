using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public class DashboardClient : IDashboardClient
{
	private readonly string FormController = "Dashboard";

	public async Task<DataTable> GetListDashboard()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetListDashboard");
	}

	public async Task<List<int>> GetListDashboardByUser(string userName)
	{
		using HttpClientHelper<List<int>> hp = new HttpClientHelper<List<int>>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetListDashboardByUser", new { userName });
	}
}
