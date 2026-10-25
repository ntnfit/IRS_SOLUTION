using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IInventoryCounting
{
	DataTable LoadData(int id, string whs, string startFrom, string startTo, string status);

	DataSet GetDataById(int Id);

	HttpResult Save(DataSet ds, string action);

	string CheckBatchSerNum(string itemCode);

	int GetMaxDocEntry();

	HttpResult UpdateStatus(int docEntry, string status);

	DataTable FindNextLastFrist(int docId, int type);

	DataTable GetOnHandByItem(string itemCode, string whs);
}
