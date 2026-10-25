using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class InventoryTransferClient : IInventoryTransferClient
{
	private const string Controller = "InventoryTranfer";

	public async Task<DataSet> GetDataLine(int docEntry)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InventoryTranfer");
		return await http.GetAsync("GetDataLine", new { docEntry });
	}

	public async Task<DataSet> GetDataCopyFrom(string lstDocEntry, ApzObjectTypes fromObj)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InventoryTranfer");
		return await http.GetAsync("GetDataCopyFrom", new { lstDocEntry, fromObj });
	}
}
