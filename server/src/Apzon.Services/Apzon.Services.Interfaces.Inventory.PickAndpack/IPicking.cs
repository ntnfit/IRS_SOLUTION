using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.PickAndpack;

public interface IPicking
{
	DataSet LoadBatchBarcode(string barcode, string itemcode, string whs, int docNo, int docRow, int line);

	DataSet LoadSalesOrder(DataTable dtSearch);

	DataSet LoadPickingList(DataTable dtSearch);

	DataSet LoadPicked(DataTable dtSearch);

	DataSet LoadPickedLine(int docentry);

	DataSet LoadBatchLine(int docEntry, string barcode, string type);

	DataTable GetDoc(DataTable pr);

	DataTable GetLines(int docEntry);

	int GetNextId();

	DataTable GetLineByPickNo(int id);

	DataSet ChoosePickList();

	HttpResult Save(DataSet ds);

	DataTable GetLineByPackNo(int id);

	DataSet ChoosePacking();

	DataSet ChooseBatch();

	HttpResult Close(int docEntry);

	HttpResult Cancel(int docEntry);

	DataSet ChooseList(ApzObjectTypes type);

	DataTable Copy(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lstEntry);

	DataTable GetBatch(int absEntry, int pickentry);

	HttpResult PickedConfirm(DataTable dt);

	HttpResult UnpickedConfirm(DataTable dt);

	DataTable AutoSelectBatch(string itemcode, string whsCode, double quantity, int linenum, int docentry, int orderNum, int orderLine);

	DataSet CreateNewInstance();

	DataTable GetConfirmed(int absEntry, int pickentry);

	DataSet GetListBatchSerial(string itemCode, string whsCode, int pickentry, int docEntry, int orderNum, int orderLine);

	HttpResult SaveList(List<DataSet> model);
}
