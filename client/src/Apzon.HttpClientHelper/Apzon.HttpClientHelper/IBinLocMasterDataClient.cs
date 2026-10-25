using System.Data;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public interface IBinLocMasterDataClient
{
	DataTable GetDataBinLocation(int absEntry);

	DataTable GetDataBinLocation(string binCode, string whsCode);

	DataTable GetBatchBinAllowcateLineByDocEntry(string docEntry, ApzObjectTypes objectType);

	DataTable GetDataCopyBinAllowcationLine(string lstDocentry, ApzObjectTypes objectType);

	DataTable GetAllBinCode();

	DataTable GetBinDefault(string tranType, string whsCode, string itemCode, double qty);

	DataTable GetBinDefaultBatch(string tranType, string whsCode, string itemCode, int absEntry, double qty);
}
