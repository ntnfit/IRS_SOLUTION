using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IWarrantyCardClient
{
	Task<DataTable> LoadData(int id, string warrantyNo, string itemCode, string bpCode, string status);

	Task<DataTable> GetDataById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<int> GetMaxId();

	Task<DataTable> FindNextLastFrist(int id, int type);
}
