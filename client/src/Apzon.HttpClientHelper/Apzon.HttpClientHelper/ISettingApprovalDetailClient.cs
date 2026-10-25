using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ISettingApprovalDetailClient
{
	Task<DataTable> LoadData(int id, string fromDate, string toDate, string objectType);

	Task<DataTable> GetAllLineById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<DataTable> GetDataHeaderById(int Id);
}
