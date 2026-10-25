using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class AboutRangeTimeClient : IAboutRangeTimeClient
{
	protected const string Controller = "AboutRangeTime";

	public async Task<DataTable> GetData()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AboutRangeTime");
		return await hp.GetAsync("GetData");
	}

	public async Task<HttpResult> SaveData(DataTable dtSave)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AboutRangeTime");
		return await hp.PostAsync("SaveData", dtSave);
	}

	public async Task<HttpResult> Delete(int docEntry)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AboutRangeTime");
		return await hp.PostAsync("Delete", docEntry);
	}
}
