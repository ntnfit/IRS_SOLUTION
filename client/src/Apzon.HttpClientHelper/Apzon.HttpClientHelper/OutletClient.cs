using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class OutletClient : IOutletClient
{
	private const string FormController = "Outlet";

	public async Task<DataTable> GetList()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Outlet");
			return await helper.GetDataTableAsync("GetList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
