using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Service;

public interface ISubActivities
{
	DataTable GetAll(string userCode);

	DataTable GetByCode(int clgCode, string userCode);

	HttpResult GetNotesByClgCode(string clgCode);

	DataTable GetBySeriesNum(int seriesNum, string userCode);

	DataTable GetDocByResource(string resourceId, string userCode);

	DataTable GetDocByAction(string action, string userCode);

	DataTable GetDocByInterval(DataTable dtSearch);

	DataTable DMSGetDocByInterval(DataTable dtSearch);

	string SaveActivityInterval(DataTable dt);

	DataTable GetCalendarResources(string user);

	DataTable SearchByIntervalMobile(DataTable dtSearch);

	DataTable LoadTask(string userName, string taskStatus = "");

	DataTable LoadTaskAndSubTask(string userName, ApzObjectTypes objType, string objId);

	DataTable LoadTaskByObjectType(string userName, ApzObjectTypes objType, string objId);

	DataTable LoadSingleTask(int clgCode, string userName);

	DataTable LoadSubTask(int clgCode, string userName);

	DataTable LoadSubNode(int clgCode, string userName);

	DataTable LoadParentTask(int parentTask);

	DataTable LoadTaskManagementSetting(string userSign);

	string GetCardTypeByCardCode(string cardCode);

	HttpResult AddTask(DataSet dt);

	string RemoveTask(DataTable dt);

	HttpResult SaveTask(DataSet dt);

	HttpResult ChangeTaskStatus(DataSet dt);

	HttpResult SaveSubTaskDetails(DataSet dt);

	HttpResult SaveSubTaskAssigner(DataSet dt);

	HttpResult SaveTaskPosition(DataSet dt);

	HttpResult SaveTaskManagementSetting(DataSet dt);

	DataTable LoadNote(string userCode, int projectCode);

	DataTable LoadNoteByObjectType(string userName, ApzObjectTypes objTypeNote, ApzObjectTypes objType, string objId);

	HttpResult SaveNote(DataSet dt);

	string DeleteNote(DataTable dt);

	DataTable LoadNoteByCode(int clgCode);

	DataTable LoadNoteByInterval(DataTable dt);

	string PushAppNotification(string noteId);

	DataTable LoadEvent(string userCode, int projectCode);

	DataTable LoadEventByObjectType(string userName, ApzObjectTypes objTypeEvent, ApzObjectTypes objType, string objId);

	DataTable LoadEventByCode(int clgCode, string userCode);

	DataTable LoadEventByInterval(DataTable dt);

	DataTable LoadEventByOpportunity(ApzObjectTypes objType, int oprId, string bpCode);

	HttpResult AddEvent(DataSet dt);

	string RemoveEvent(DataTable dt, int type);

	HttpResult SaveEvent(DataSet dt, int type);

	DataSet LoadComment(int Id, string type, int userSign);

	HttpResult SaveComment(Tuple<DataTable, List<int>> dt);

	string ClearUnreadComment(DataTable dt);

	string DeleteComment(int id);

	string RemoveCommentById(int Id, string type);

	HttpResult MarkAsRead(int comment);

	DataTable GetUnlockedUsers();

	DataTable GetActiveEmployees();

	DataTable GetAllNotification(int userId);

	DataTable GetDataById(int id);

	string GetUserComment(string objType, int id);

	DataTable GetStruct();

	HttpResult AddMuntiTask(DataTable dt);

	DataTable GetEmployeeByUserId(int userId);

	string CheckExists(int oclg_code);
}
