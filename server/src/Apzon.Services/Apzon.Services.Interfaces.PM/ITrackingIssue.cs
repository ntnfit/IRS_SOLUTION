using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PM;

public interface ITrackingIssue
{
	DataTable Search(string bpCode, string projectCode, int projectOwner, string reportBy, string handleBy, int id, string status, string fromDate, string toDate, string userName = "");

	DataSet GetDataById(int Id);

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	string DeleteDocs(string Id);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);

	HttpResult SaveSearch(DataSet ds, string userId);

	HttpResult SaveSearchData(DataSet ds, ref string objId, ref string objNum);

	DataTable GetDataTypeById(string Id);

	DataTable GetDataSearchByUserId(int userId, string formId);

	DataTable LoadIssueByProjectId(int projectId);

	DataTable LoadIssueByCallId(int callId);

	DataTable GetEmployeeByUserId(int userId);

	HttpResult UpdateStatus(DataSet ds);

	HttpResult UpdateStatusData(DataSet ds, ref string objId, ref string objNum);

	string GetUseHandleById(int id);

	string GetUseReportById(int id);

	DataSet GetStructure();

	string GetUserHistoryAssign(int issueId);

	DataTable GetDataStatusById(string Id);

	DataTable GetStructureAtc1();

	DataTable GetAllStatus();

	DataTable GetFileByOjectType(string objectType, string objectCode);

	DataTable GetAllUser();
}
