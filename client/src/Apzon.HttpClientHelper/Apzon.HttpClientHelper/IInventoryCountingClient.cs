using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IInventoryCountingClient
{
	Task<DataTable> LoadData(int id, string whs, string startFrom, string startTo, string status);

	Task<DataSet> GetDataById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> CheckBatchSerNum(string itemCode);

	Task<int> GetMaxDocEntry();

	Task<HttpResult> UpdateStatus(int docEntry, string status);

	Task<DataTable> FindNextLastFrist(int docId, int type);

	Task<DataTable> GetOnHandByItem(string itemCode, string whs);
}
