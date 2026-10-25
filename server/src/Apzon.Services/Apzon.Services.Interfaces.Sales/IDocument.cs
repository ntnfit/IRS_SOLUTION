using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Sales;

public interface IDocument : IService<DocumentsImpl>
{
	DataTable GetDocLinesByListDocEntry(List<int> docEntry, ApzObjectTypes objtype);

	DataTable TableLine(int docentry, ApzObjectTypes objtype, string filter = "");

	DataTable DMSTableLine(string docentry, ApzObjectTypes objtype, string filter = "");

	DataTable Search(DocumentsImpl doc, ApzObjectTypes objtype);

	DataTable DMSSearch(DMSSaleImpl doc, ApzObjectTypes objtype);

	int GetMaxDocEntry(ApzObjectTypes objtype);

	int GetMaxDocNum(ApzObjectTypes objtype);

	int GetMinDocEntry(ApzObjectTypes objtype);

	DataTable GetListBpCatalog(ApzObjectTypes objtype);

	DataTable Duplicate(int docEntry, ApzObjectTypes objtype);

	DataTable GetFieldByItemCode(string itemCode, ApzObjectTypes objtype);

	DataTable GetPayment(int docEntry, ApzObjectTypes objtype);

	DataTable LoadBatchNumberStruct(ApzObjectTypes objtype);

	DataTable GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode, string objType);

	DataTable GetBatchSerialByDocEntry(int docEntry, ApzObjectTypes objtype);

	DataTable CopyBatchSerial(string fromentry, string fromtype);

	DataTable LoadSeries(string usercode, ApzObjectTypes objtype);

	int GetDocumentNumber(int series, ApzObjectTypes objtype);

	HttpResult SaveDataSet(DataSet ds, ApzObjectTypes objtype);

	HttpResult SaveAsDraftDataSet(DataSet doc);

	HttpResult CloseDoc(int docEntry, ApzObjectTypes objtype);

	DataTable GetFreight(int docEntry, ApzObjectTypes objtype);

	DataTable GetFreightDefault(ApzObjectTypes objtype);

	DataTable GetTaxGroup(ApzObjectTypes objtype);

	DataTable GetFreightCode(ApzObjectTypes objtype);

	DataTable GetRateByVatGroup(ApzObjectTypes objtype);

	HttpResult CancelDoc(int docEntry, ApzObjectTypes objtype);

	DataTable CopyFromLine(string type, string entry, string to);

	DataTable CopyFrom(string type, string entry, string to, int bplId, string username);

	DataTable GetExpnsByCode(ApzObjectTypes objtype, string expnsCode);

	HttpResult Remove(int docEntry, ApzObjectTypes objtype);

	DataTable GetLineShipTo(string cardCode);

	int GetDocNumByDocEntryType(int baseKey, int baseType);

	DataTable GetOwnerByBpUser(string cardCode, int userID);

	int GetDocTypeByDraftKey(int draftKey);

	void BatchAndSerialNumberProcess(DataSet ds, string objId, ApzObjectTypes objType, Enums.TransactionType transtype, string objNum);

	int GetMaxDocEntrySaveAsDraf();

	int InsertDocumentApprovalRequest(SqlParameter[] paramater);

	bool SaveDraftToDocument(ApzObjectTypes objectTypes, int docEntry, int draftKey);

	string GetWddStatus(ApzObjectTypes objType, int docEntry);

	int GetDocEntryByDocNumType(int baseKey, int baseType);

	DataTable GetDownPaymentToDraw(ApzObjectTypes objType, string cardCode, string docCur);

	DataTable GetDownPaymentDrawed(ApzObjectTypes objType, int docEntry);

	DataTable GetReferencedDocument(ApzObjectTypes formObjectTypes, int docEntry);

	DataTable ListChooseDocuments(ApzObjectTypes formObjectTypes, string from, string to);

	DataTable GetReferencedDocumentBy(ApzObjectTypes formObjectTypes, int docEntry);
}
