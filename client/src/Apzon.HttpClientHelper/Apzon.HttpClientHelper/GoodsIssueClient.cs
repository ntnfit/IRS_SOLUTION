using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class GoodsIssueClient : IGoodsIssueClient
{
	private const string FormController = "GoodsIssueIt";

	public async Task<HttpResult> SaveDoc(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodsIssueIt");
		return await helper.PostAsync("SaveDoc", doc);
	}
}
