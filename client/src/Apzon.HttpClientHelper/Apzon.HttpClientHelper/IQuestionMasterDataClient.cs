using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IQuestionMasterDataClient
{
	Task<DataTable> LoadData(int id, string fromDate, string toDate, string status);

	Task<DataTable> GetDataById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<int> GetNo();

	Task<DataTable> FindNextLastFrist(int id, int type);
}
