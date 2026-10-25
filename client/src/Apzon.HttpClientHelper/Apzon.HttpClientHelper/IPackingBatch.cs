using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPackingBatch
{
	Task<DataSet> GetBatch(string batchNo);

	Task<int> GetPage(int current, int type, string username, int branch);

	Task<DataSet> GetIssue(int id);

	Task<DataSet> GetReceipt(int id);

	Task<DataSetResult> GetDataByBarcode(string barcode, string boxCode);
}
