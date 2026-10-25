using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IStockCounting
{
	DataTable SearchDataByWarehouse(string whs);

	DataTable SearchDataByItemCode(string whs, string itemCode);

	DataTable GetTableLine(int id);

	DataTable GetNo();

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	int GetNextNumber(int series);

	DataTable FindNextLastFrist(int docId, int type);

	int GetNextNumberDraft(int series);

	DataTable GetNoDraf();

	HttpResult SaveDraft(DataSet ds, string action);

	HttpResult SaveDataDraft(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);
}
