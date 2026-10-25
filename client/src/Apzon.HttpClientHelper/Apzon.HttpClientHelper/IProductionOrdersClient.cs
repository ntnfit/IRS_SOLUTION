using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;

namespace Apzon.HttpClientHelper;

public interface IProductionOrdersClient
{
	Task<HttpResult> SaveList(List<ProductionOder_Impl_OWOR> oDocs);

	Task<HttpResult> UpdateStatus(List<ProductionOder_Impl_OWOR> oDocs);

	Task<HttpResult> ReceiptForBatch(Tuple<int, DocumentsImpl, DocumentsImpl, string, string> tuple);

	Task<HttpResult> PackingBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> tuple);

	Task<HttpResult> ConfirmBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> tuple);

	Task<HttpResult> SaveProductionPlan(List<ProductionOder_Impl_OWOR> oDocs, DocumentsImpl tran, DataTable dtOperation);

	Task<int> GetDocNum(int series);

	Task<DataTable> GetByBaseEntry(int baseEntry, ApzObjectTypes objecttype);

	Task<HttpResult> SaveDataSet(DataSet ds);

	Task<DataTable> GetRecord(string position, string docEntry);

	Task<DataTable> FindDataNextLast(int id, int type);

	Task<DataTable> GetADoc(int docEntry);

	Task<DataSet> SearchLine(ProductionOder_LineImpl_WOR1 line);

	DataTable GetItemDetails(string type, string code);

	Task<string> CancelDoc(int _currentDocEntry);

	Task<DataTable> Commons_GetDataBrach(string userCode);

	DataTable Commons_GetSeries(int ObjType);

	Task<DataTable> ShowUsers();

	Task<DataTable> GetActiveDimensionCount();

	Task<DataTable> GetItem_ITT1(BillOfMasterials_LineImpl_ITT1 line);

	DataTable GetOITWQuantity(string code, string whsCode);

	Task<DataTable> GetItemNameUoM(string itemCode);

	DataTable GetSalesOrderEntry(int docNum);

	Task<HttpResult> SaveDoc(ProductionOder_Impl_OWOR doc);

	Task<DataTable> Search(DataSet ds);

	Task<DataTable> GetIsIssued(int docEntry);

	Task<DataSet> GetIssueComponents(int docEntry);

	Task<DataSet> GetTransferComponents(int docEntry, double parentQty);

	Task<HttpResult> CheckBaseProduction(int entry, ApzObjectTypes objtype);

	Task<DataSetResult> GetDataByBarcode(string barcode, string userName, int branch);

	Task<DataTable> CheckByDocNum(string type, int docNum);

	DataTable GetReferencedDocument(string FromDocEntry);
}
