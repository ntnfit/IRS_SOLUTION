using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Purchase;

public interface IPurchaseRequest : IService<DocumentsImpl>
{
	string SaveAsDraft(DocumentsImpl model, ApzObjectTypes apzObjectTypes);

	DataTable SelectOne(int docEntry, bool isDraft = false);

	DataTable SearchDocments(DocumentsImpl doc, string filter = "");

	DataTable GetDepartment(string requester);

	DataTable GetBranch(string requester);

	int GetMaxDocEntry();

	int GetMinDocEntry();

	bool CheckDoc(int docEntry);

	bool CheckDocLine(int docEntry, int lineNum);

	int GetQuantityBase(string tbl, string tblLine, int baseEntry, int baseLine);

	int GetOpenQuantity(string tbl, string tblLine, int docEntry, int lineNum);

	int DealDocOrgin(ApzObjectTypes typeFrom, int baseEntry);

	int DealLineOrgin(ApzObjectTypes typeTo, ApzObjectTypes typeFrom, int baseEntry, int baseLine);

	List<string> GetTableName(ApzObjectTypes type);

	DataTable GetListDepartment();

	DataTable GetListBranch();

	DataTable GetListPurchaseRequest();

	DataTable GetRecoderDoc(int id);

	DataTable GetRecoderLine(int id);

	DataTable GetDefaultTableDoc();

	DataTable GetDefaultTableLine();

	DataTable GetFirstRecord();

	DataTable GetLastRecord();

	DataTable GetNextRecord(string id);

	DataTable GetPreviousRecord(string id);

	int GetMaxDocNum();

	DataTable GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode, string objType);

	DataTable GetBatchSerialByDocEntry(int docEntry);

	DataTable CopyBatchSerial(string fromentry, string fromtype);

	DataTable LoadSeries(string usercode);

	int GetDocumentNumber(int series);
}
