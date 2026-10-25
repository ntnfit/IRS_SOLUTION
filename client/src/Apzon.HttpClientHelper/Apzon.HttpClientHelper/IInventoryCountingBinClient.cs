using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IInventoryCountingBinClient
{
	Task<DataSet> TransferGet(string batchNo, string bin);

	Task<DataTable> LoadAllBin(string itemCode, string whsCode, int batSerialAbs, int tranType, string binCode);

	Task<DataSet> GetBinInstock(string bin, string period);

	Task<DataTable> GetPeriods();

	Task<DataTable> CheckBatch(string batch, string bin, string period);

	Task<DataSet> GetById(int id);

	Task<DataSet> GetCountingReport(string period, string bin);

	Task<HttpResult> Posting(DocumentsImpl oTransfer, DocumentsImpl oReceipt, DocumentsImpl oIssue);

	Task<DataSetResult> GetDataByBarcode(string batchNo, string whs, string fromWhs, int requestId);

	Task<DataTableResult> GetWarehouseRequest(string docEntry);
}
