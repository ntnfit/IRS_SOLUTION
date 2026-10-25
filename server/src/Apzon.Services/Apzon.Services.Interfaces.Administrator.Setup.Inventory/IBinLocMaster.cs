using System.Data;
using Apzon.Commons;

namespace Apzon.Services.Interfaces.Administrator.Setup.Inventory;

public interface IBinLocMaster
{
	DataTable GetBinLocByBinCode(string binCode);

	DataTable GetWHSublevel();

	DataTable GetDataBinLocation(int absEntry);

	DataTable GetDataBinLocation(string binCode, string whsCode);

	DataTable GetBatchBinAllowcateLineByDocEntry(string docEntry, ApzObjectTypes objectType);

	DataTable GetDataCopyBinAllowcationLine(string lstDocentry, ApzObjectTypes objectType);

	DataTable GetAllBinCode();

	DataTable GetTableStructure();

	DataTable GetBinLocProperties(int absEntry);

	string GetBinSeperatorOfWarehouse(string whsCode);

	DataTable GetBinDefault(string tranType, string whsCode, string itemCode, double qty);

	DataTable GetBinDefaultBatch(string tranType, string whsCode, string itemCode, int absEntry, double qty);
}
