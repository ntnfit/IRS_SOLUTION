using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IInventoryCoutingBin
{
	DataTable SearchDataByBarCode(string barcode, string whs, string bin);

	string GetRecord(int docEntry, int type);

	DataSet TransferGet(string batchNo, string bin);

	DataTable LoadAllBin(string itemCode, string whsCode, int batchSerialAbs, int tranType, string binCode);

	DataSet GetBinInstock(string bin, string period);

	DataTable GetPeriods();

	DataTable CheckBatch(string batch, string bin, string period);

	int GetPage(int current, int type);

	DataSet GetById(int id);

	DataSet GetCountingReport(string period, string bin);

	DataSetResult GetDataByBarcode(string barcode, string whs, string fromWhs, int requestId);

	DataSetResult GetDataSummaryByBarcode(string barcode, string whs, string fromWhs);

	DataTableResult GetWarehouseRequest(string docEntry);
}
