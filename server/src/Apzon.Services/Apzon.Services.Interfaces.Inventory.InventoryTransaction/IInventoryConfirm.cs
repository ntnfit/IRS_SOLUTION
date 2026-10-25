using System.Data;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IInventoryConfirm
{
	DataSet SearchDataByBarCode(string barcode, string whs);

	string GetRecord(int docEntry, int type);

	DataSet GetLineItem(int docEntry);

	int GetMaxDocEntry();
}
