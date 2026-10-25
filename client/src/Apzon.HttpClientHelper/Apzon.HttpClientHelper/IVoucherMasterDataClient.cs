using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IVoucherMasterDataClient
{
	Task<DataTable> GetAllVoucher();

	Task<DataTable> FindDataNextLast(string id, int type);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<DataTable> GetDetailByCode(string pinCode);

	Task<string> DeleteDocs(string ids);
}
