using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IInventoryTranfer
{
	int GetMaxDocEntry();

	DataTable Search(DocumentsImpl doc);

	DataTable SearchLine(Document_LinesImpl line);

	string SaveInventoryTransferRequestDetail(string docs);

	string SaveDoc(DataSet doc);

	string DeleteLine(int docEntry, int lineNum);

	string CancelLine(string docentry, string lineNum, string quatity, string itemcode, string whscode, string towhscode, string linetotal);

	int GetNewDocNum();

	string CloseDoc(int docEntry);

	string CancelDoc(int docEntry);

	DataSet GetDataCopyFrom(string lstDocEntry, ApzObjectTypes fromObj);

	DataTable GetDataDoc(string cardCode, int docNum, int docEntry, string docStatus, int prilist, string fromDate, string toDate, Enums.NavigateType type);

	DataSet GetDataLine(int docEntry);

	int GetDocNumByDocEntry(int docEntry);

	DataSet GetItemByBarcode(string barcode, string type);

	DataTable GetWhsByBin(string binCode);

	string CreatePalletNo(int binId);

	HttpResult AddTransferMobileAppLog(DataTable dtTransfer, int transferEntry);
}
