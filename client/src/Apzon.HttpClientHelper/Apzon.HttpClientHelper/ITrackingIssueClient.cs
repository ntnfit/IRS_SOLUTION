using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ITrackingIssueClient
{
	Task<DataTable> Search(string bpCode, string projectCode, int projectOwner, string reportBy, string handleBy, int id, string status, string fromDate, string toDate, string userName = "");

	Task<DataSet> GetDataById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<HttpResult> SaveSearch(DataSet ds, string userId);

	Task<DataTable> GetDataTypeById(string Id);

	Task<DataTable> GetDataSearchByUserId(int userId, string formId);

	Task<DataTable> LoadIssueByProjectId(int projectId);

	Task<DataTable> LoadIssueByCallId(int CallId);

	Task<DataTable> GetEmployeeByUserId(int userId);

	Task<HttpResult> UpdateStatus(DataSet ds);

	Task<DataSet> GetStructure();

	Task<DataTable> GetDataStatusById(string Id);

	Task<DataTable> GetAllStatus();

	Task<DataTable> GetFileByOjectType(string objectType, string objectCode);
}
