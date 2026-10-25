using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICashItemMasterDataClient
{
	Task<DataTable> GetAllCash();

	Task<DataTable> FindDataNextLast(string id, int type);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<DataTable> GetDetailByCode(string cashCode);

	Task<string> DeleteDocs(string ids);
}
