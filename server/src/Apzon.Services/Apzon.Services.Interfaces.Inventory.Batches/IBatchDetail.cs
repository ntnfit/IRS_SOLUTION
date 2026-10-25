using System.Data;

namespace Apzon.Services.Interfaces.Inventory.Batches;

public interface IBatchDetail
{
	DataTable Search(string itemCode, string itemName, string batch);

	DataTable GetQuantities(string itemCode, int sysNumber);

	string Save(DataSet ds);

	DataTable GetByAbsEntry(int absEntry);

	DataTable LoadAnEntry(int absEntry);

	DataTable GetDataBatchWhs(string itemCode, string code, string whs);

	DataTable GetDataAutoSelect(string type, string isAllocateBin, string itemCode, string whsCode, double genQty, DataTable dtExistsBatch);

	DataTable GetDataAllowcateBin(string itemCode, string whsCode, decimal qty);

	DataSet LoadDataAutoSelectConfig();

	string SaveConfig(DataTable dt, string userName);

	int GetPage(int current, int type, string username, int branch);

	DataSet GetBatchDetail(int absEntry, string itemCode, string status, string distNumber, string mnfSerial, string lotNumber);

	DataTable GetDataBatchSerial(string serialNo, string mnfSerial, string lotNumber, string itemCode, string tranType);
}
