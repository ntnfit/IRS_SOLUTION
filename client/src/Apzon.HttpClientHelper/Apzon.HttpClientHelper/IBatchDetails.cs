using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IBatchDetails
{
	DataTable GetByAbsEntry(int absEntry);

	Task<DataTable> GetDataBatchWhs(string itemCode, string code, string whs);

	Task<DataTable> GetDataAutoSelect(string type, string isAllocateBin, string itemCode, string whsCode, double genQty, DataTable dtExistBatch);

	Task<DataSet> LoadDataAutoSelectConfig();

	Task<string> SaveConfig(DataTable dt);

	Task<int> GetPage(int currentId, int type, string username, int branch);

	Task<DataSet> GetBatchDetail(int absEntry, string itemCode = "", string status = "", string distNumber = "", string mnfSerial = "", string lotNumber = "");

	Task<string> Save(DataSet ds);

	DataTable GetDataAllowcateBin(string itemCode, string whsCode, decimal qty);

	DataTable GetDataBatchSerial(string serialNo, string mnfSerial, string lotNumber, string itemCode, string tranType);
}
