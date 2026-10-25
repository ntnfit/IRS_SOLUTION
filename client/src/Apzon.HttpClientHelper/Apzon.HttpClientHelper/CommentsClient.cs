using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class CommentsClient : ICommentsClient
{
	public const string Controller = "Comments";

	public async Task<HttpResult> AddNew(DataTable dt)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Comments");
		return await hp.PostAsync("AddNew", dt);
	}

	public async Task<DataTable> GetByObject(string objectType, string objectCode, string objectDetail)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Comments");
		return await hp.GetAsync("GetByObject", new { objectType, objectCode, objectDetail });
	}
}
