using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;

namespace Apzon.HttpClientHelper;

internal class CapacityClient : ICapacityClient
{
	private const string FormController = "Capacity";

	public async Task<DataTable> GetResources()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Capacity");
		return await hp.GetAsync("GetResources");
	}

	public async Task<DataTable> GetDataAppointments(int scheduleId, Enums.TransactionType transactionType = Enums.TransactionType.A)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Capacity");
		return await hp.GetAsync("GetDataAppointments", new { scheduleId, transactionType });
	}

	public async Task<DataTable> GetColorSetting()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Capacity");
		return await hp.GetAsync("GetColorSetting");
	}

	public async Task<string> Save(DataTable dt)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "Capacity");
		return await hp.PostAsync("Save", dt);
	}

	public DataTable GetDataResourceEffic(string resCode, string routCode, int routLine)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Capacity");
		return httpClientHelper.Get("GetDataResourceEffic", new { resCode, routCode, routLine });
	}
}
