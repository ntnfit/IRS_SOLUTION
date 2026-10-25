using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ISerialDetails
{
	DataTable GetByAbsEntry(int absEntry);

	Task<int> GetPage(int currentId, int type, string username, int branch);

	Task<DataSet> GetSerialDetail(int absEntry, string itemCode = "", string status = "", string distNumber = "", string mnfSerial = "", string lotNumber = "");

	Task<string> Save(DataTable dtSave);
}
