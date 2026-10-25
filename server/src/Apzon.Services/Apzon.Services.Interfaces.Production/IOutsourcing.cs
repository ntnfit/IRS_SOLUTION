using System.Data;

namespace Apzon.Services.Interfaces.Production;

public interface IOutsourcing
{
	DataTable Search(DataTable dtSearch);

	DataTable AddNewRow(string grid);

	DataTable GetItemDetails(string itemCode);

	DataTable GetBatchSerials(string docEntry, string objType);

	DataTable GetBatchSerialsColumns();

	DataTable GetAllBinData(string itemCode, string whsCode);

	DataTable GetBOMComponents(DataTable itemCodes);

	DataTable GetTotalAvgPrice(DataTable strItemCodeWarehouses);

	DataTable GetADoc(int docNum, string grid);

	int GetRecord(string position, int code);

	bool ExistsCode(int code);

	DataTable GetItemsAvgPrice(DataTable dtItemWhs);

	DataTable GetBinData(string code, int type, string itemCode);
}
