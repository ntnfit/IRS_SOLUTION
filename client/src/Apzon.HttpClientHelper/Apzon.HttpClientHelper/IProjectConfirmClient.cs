using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IProjectConfirmClient
{
	Task<DataTable> LoadData(int id, string projectCode, string startFrom, string startTo, string status);

	Task<DataTable> GetDataById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<int> GetNo();

	Task<HttpResult> UpdateStatus(int docEntry, string status);

	Task<DataTable> FindNextLastFrist(int id, int type);
}
