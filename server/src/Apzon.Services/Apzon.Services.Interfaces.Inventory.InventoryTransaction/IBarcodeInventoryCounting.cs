using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IBarcodeInventoryCounting
{
	DataSet GetDataStructure();

	DataTable SearchDoc(DataTable dtSearch);

	DataTable GetDataLineByDocEntry(int docEntry);

	int GetNewDocEntry();

	DataTable GetNextLastData(Enums.NavigateType type, int docEntry);

	DataTable GetDataByBarcode(string barcodeString, string whsCode, string binCode);

	HttpResult AddNew(DataSet dsSave);

	HttpResult UpdateData(DataSet dsSave);

	string Delete(int docEntry);
}
