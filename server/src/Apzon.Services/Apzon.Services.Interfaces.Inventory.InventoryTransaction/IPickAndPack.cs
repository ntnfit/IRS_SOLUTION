using System.Data;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IPickAndPack
{
	DataSet GetBatchItem(string barcode);

	DataSet GetBatchDocument(string barcode);

	DataSet GetRecord(int docEntry, string type);

	int GetMaxDocEntry(string tableName);

	DataSet GetLineItem(int docEntry);
}
