using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class InventoryTransferRequestClient : IInventoryTransferRequestClient
{
	private const string FormController = "TranferRequest";

	public async Task<HttpResult> SaveDoc(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TranferRequest");
		return await helper.PostAsync("SaveDoc", doc);
	}

	public async Task<DataSet> GetDataCopyFrom(string docEntry, ApzObjectTypes fromType)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TranferRequest");
		return await helper.GetAsync("GetDataCopyFrom", new
		{
			docEntry = docEntry,
			fromObj = fromType
		});
	}

	public async Task<HttpResult> SaveAsDraft(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TranferRequest");
		return await helper.PostAsync("SaveAsDraft", doc);
	}
}
