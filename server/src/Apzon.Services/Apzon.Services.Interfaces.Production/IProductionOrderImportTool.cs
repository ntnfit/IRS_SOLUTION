using System.Data;

namespace Apzon.Services.Interfaces.Production;

public interface IProductionOrderImportTool
{
	DataTable Search(DataTable dt);

	DataTable GetTableStructure(string tabName);

	DataTable GetBOMHeader(string code);

	DataTable GetItemCodeAndName();

	DataTable RCGetProductionDetails(string barCode);

	DataTable GetTransferedBinData(int docEntry, int lineNum);

	DataTable GetAllBinData(string itemCode, string whsCode);

	bool IsBinWarehouse(string whsCode);

	DataTable GetBinWarehouse();

	DataTable GetAllBatchSerial(string itemCode, string fromWhsCode, string toWhsCode);

	DataTable GetEditTableEnableColumns();

	string UpdateDataForBarcodeApp(string weekNo);

	DataSet IssNdRecQtySearch(string date);
}
