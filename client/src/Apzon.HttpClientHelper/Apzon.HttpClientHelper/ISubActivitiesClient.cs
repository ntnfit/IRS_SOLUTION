using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ISubActivitiesClient
{
	Task<DataTable> LoadTask(string userName, string taskStatus = "");

	Task<DataTable> LoadTaskAndSubTask(string userName, ApzObjectTypes objType, string objId);

	Task<DataTable> LoadTaskByObjectType(string userName, ApzObjectTypes objType, string objId);

	Task<DataTable> LoadSingleTask(int clgCode, string userName);

	DataTable LoadSingleTaskSync(int clgCode, string userName);

	Task<DataTable> LoadSubTask(int clgCode, string userName);

	Task<DataTable> LoadSubNode(int clgCode, string userName);

	Task<DataTable> LoadParentTask(int parentTask);

	Task<DataTable> LoadTaskManagementSetting(string userSign);

	string GetCardTypeByCardCode(string cardCode);

	Task<HttpResult> SaveTask(DataSet dt);

	Task<HttpResult> ChangeTaskStatus(DataSet dt);

	Task<HttpResult> AddTask(DataSet dt);

	Task<string> RemoveTask(DataTable dt);

	Task<HttpResult> SaveSubTaskDetails(DataSet dt);

	Task<HttpResult> SaveSubTaskAssigner(DataSet dt);

	Task<HttpResult> SaveTaskPosition(DataSet dt);

	Task<HttpResult> SaveTaskManagementSetting(DataSet dt);

	Task<DataTable> LoadEvent(string userCode, int projectCode);

	Task<DataTable> LoadEventByObjectType(string userName, ApzObjectTypes objTypeEvent, ApzObjectTypes objType, string objId);

	Task<DataTable> LoadEventByCode(int clgCode, string userCode);

	Task<DataTable> LoadEventByInterval(DataTable dt);

	Task<DataTable> LoadEventByOpportunity(ApzObjectTypes objType, int oprId, string bpCode);

	Task<HttpResult> SaveAllEvent(DataSet ds);

	Task<HttpResult> SaveThisEvent(DataSet ds);

	Task<HttpResult> AddEvent(DataSet ds);

	Task<string> RemoveAllEvent(DataTable dt);

	Task<string> RemoveUnchangedEvent(DataTable dt);

	Task<DataTable> LoadNote(string userCode, int projectCode);

	Task<DataTable> LoadNoteByObjectType(string userName, ApzObjectTypes objTypeNote, ApzObjectTypes objType, string objId);

	Task<HttpResult> SaveNote(DataSet dt);

	Task<string> DeleteNote(DataTable dt);

	Task<DataTable> LoadNoteByCode(int clgCode);

	Task<DataTable> LoadNoteByInterval(DataTable dt);

	Task<DataSet> LoadComment(int Id, string type, int userSign);

	Task<HttpResult> SaveComment(DataTable dt, List<int> additionNotifyUsers);

	Task<string> DeleteComment(int id);

	Task<string> RemoveCommentById(int _id, string type);

	Task<HttpResult> MarkAsRead(int comment);

	Task<string> PushAppNotification(string noteId);

	Task<DataTable> GetDoc(string userCode);

	Task<DataTable> GetDocByResoucre(string resourceId, string userCode);

	Task<DataTable> GetDocByAction(string action, string userCode);

	Task<DataTable> GetActivityByCode(int clgCode, string userCode);

	Task<DataTable> GetActivityBySeriesNum(int seriesNum, string userCode);

	Task<string> SaveActivityInterval(DataTable dt);

	Task<DataTable> GetUnlockedUsers();

	Task<DataTable> GetActiveEmployees();

	Task<DataTable> GetDocByInterval(DataTable dt);

	Task<DataTable> GetCalendarResources(string user);

	Task<DataTable> GetAllNotification(int userId);

	Task<string> ClearUnreadComment(DataTable dtUnread);

	Task<DataTable> GetStruct();

	Task<HttpResult> AddMuntiTask(DataTable dt);

	Task<DataTable> GetEmployeeByUserId(int userId);

	bool CheckExists(int oclg_code);
}
