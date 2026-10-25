using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Production;

public interface IProductionOder
{
	DataTable GetMaxDocEntry();

	DataTable ShowOWOR();

	DataTable ShowWOR1();

	DataSet SearchLine(ProductionOder_LineImpl_WOR1 line);

	DataTable GetItem_ITT1(BillOfMasterials_LineImpl_ITT1 line);

	DataTable GetList_SalesOder();

	bool Exists(int docEntry);

	bool CheckLine(int docEntry, int lineNum);

	DataTable Check_FirstRecord();

	DataTable Check_LastRecord();

	DataTable GetFirstRecord();

	DataTable GetLastRecord();

	DataTable GetNextRecord(string docEntry);

	DataTable GetPreviousRecord(string docEntry);

	DataTable FindDataNextLast(int id, int type);

	DataTable GetTypeByDocEntryDataTable(string docEntry);

	DataTable GetDataItemProduction(string tranType);

	DataTable GetDataItemManualProduction(string tranType);

	DataTable GetListStandardOrders();

	DataTable GetItemDetails(string type, string code);

	DataTable Search(DataTable dataTable1, DataTable dataTable2);

	DataTable GetADoc(int docEntry);

	DataTable GetSearchTableLine();

	DataTable GetSearchTableDoc();

	DataTable ShowUsers();

	DataTable GetIsIssued(int docEntry);

	DataTable GetOITWQuantity(string code, string whsCode);

	DataTable GetItemNameUoM(string itemCode);

	HttpResult CancelDoc(int docEntry);

	DataTable ValidateDocData(DataTable dt);

	DataTable GetSalesOrderEntry(int docNum);

	DataTable GetRecord(string position, string docEntry);

	HttpResult SaveDataSet(DataSet ds);

	int GetDocNum(int series);

	DataTable GetByBaseEntry(int baseEntry, ApzObjectTypes objecType);

	string UpdateStatus(ProductionOder_Impl_OWOR obj);

	DataSet GetByKey(int docentry);

	DataTable CheckByDocNum(string type, int docNum);

	DataSet GetProductionByBarcode(string barcode);

	DataSet GetIssueComponents(int docEntry);

	DataSet GetTransferComponents(int docEntry, double parentQty);

	HttpResult CheckBaseProduction(int entry, ApzObjectTypes objtype);

	DataSetResult GetDataByBarcode(string barcode, string userName, int branch);

	DataTable GetReferencedDocument(string FromDocEntry);
}
