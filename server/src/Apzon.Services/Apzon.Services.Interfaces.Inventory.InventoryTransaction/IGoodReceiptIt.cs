using System.Collections.Generic;
using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IGoodReceiptIt : IService<DocumentsImpl>
{
	DataSet SelectOne(int idDocEntry);

	int GetMaxDocEntry();

	DataTable GetItemProductionOrder(int docentry);

	DataTable GetDepartment(string requester);

	DataTable GetBranch(string requester);

	DataTable GetInStock(string itemCode);

	DataTable GetGroupNum();

	DataTable GetListNum(string key);

	int GetNewDocNum(int series);

	HttpResult ReportComplete(DataSet dtset);

	DataTable GetItemInStockAWarehouse(List<string> itAndWh);

	DataTable GetItemInStockAllWarehouse(string itemCode);

	DataTable GetDataDoc(int baseType, int docNum, int docEntry, int priList, string fromDate, string toDate, Enums.NavigateType type, string userCode);

	DataSetResult GetDataByBarcode(string barcode, string userName, int branch);

	HttpResult GetCurrentRate(DataTable dtSearch);

	DataSetResult GetDataByDraftKey(string barcode);
}
