using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface ITranferRequest
{
	int GetMaxDocEntry();

	DataTable Search(DocumentsImpl doc);

	DataTable SearchLine(Document_LinesImpl line);

	string SaveDoc(DataSet doc);

	string DeleteLine(int docEntry, int lineNum);

	int GetNewDocNum();

	string CancelLine(string docentry, string lineNum, string quatity, string itemcode, string whscode, string towhscode, string linetotal);

	DataTable GetCopyLines(List<int> docEntries);

	string CloseDoc(int docEntry);

	DataTable GetDataDoc(string cardCode, int docNum, int docEntry, string docStatus, int prilist, string fromDate, string toDate, Enums.NavigateType type);

	DataSet GetDataLine(int docEntry);

	DataSet GetDataCopyFrom(string docEntry, ApzObjectTypes fromObj);

	DataTableResult GetDataByBarcode(string barcode, string type);

	void LogBinAllocate(DataTable dt);
}
