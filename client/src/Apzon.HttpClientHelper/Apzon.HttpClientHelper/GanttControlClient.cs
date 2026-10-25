using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class GanttControlClient : IGanttControlClient
{
	private readonly string FormController = "GanttControl";

	public async Task<DataTable> LoadByProject(int projectAbsEntry)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await helper.GetAsync("LoadByProject", new { projectAbsEntry });
	}

	public async Task<HttpResult> UpdateTask(DataTable dt)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
		return await helper.PostAsync("UpdateTask", dt);
	}
}
