using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public interface IProductionOrderImportTool
{
	DataTable GetTableStructure(string tabName);

	DataTable Search(DataTable dt);

	DataTable GetBOMHeader(string code);

	DataTable GetItemCodeAndName();

	DataTable RCGetProductionDetails(string barCode);

	Task<HttpResult> SaveFG(DataTable dt);

	Task<string> SaveFGSerial(DataSet ds);

	Task<string> SaveComp(DataTable dt);

	Task<string> SaveTransfer(DocumentsImpl model);

	Task<string> SaveRCFromPC(DataTable dtSave);

	Task<string> SaveClose(DataTable dtSave);

	Task<DataTable> GetTransferedBinData(int docEntry, int lineNum);

	Task<DataTable> GetAllBinData(string itemCode, string whsCode);

	Task<bool> IsBinWarehouse(string toWhsCode);

	Task<DataTable> GetBinWarehouse();

	Task<DataTable> GetAllBatchSerial(string itemCode, string fromWhsCode, string toWhsCode);

	DataSet GetProductionByBarcode(string barCode);

	string SaveRC(DataTable dt);

	DataTable GetEditTableEnableColumns();

	Task<HttpResult> SaveTempFG(DataTable dtSave);

	string UpdateDataForBarcodeApp(string weekNo);

	DataSet IssNdRecQtySearch(string v);
}
