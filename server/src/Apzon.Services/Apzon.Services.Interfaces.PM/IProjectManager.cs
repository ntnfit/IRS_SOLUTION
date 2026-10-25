using System.Data;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.PM;

public interface IProjectManager
{
	DataTable LoadData(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName, string filterString = "");

	DataSet GetDataById(int Id);

	DataTable GetListTask(int projectId);

	DataTable GetListTracking(int projectId);

	HttpResult Save(DataSet ds, string action);

	HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum);

	HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum);

	string DeleteDocs(string Id);

	HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum);

	DataTable LoadSubProjectByMasterId(int projectId);

	DataTable GetDataBOMSaleOrder(string code);

	DataTable GetDataItemProduction(string code);

	DataTable GetDataItemInBom(string code);

	DataTable GetDataItemGroup();

	HttpResult UpdateStatus(int docEntry, string status);

	DataTable FindNextLastFrist(int id, int type);

	DataSet GetDataLineByWbsTemplate(string code);

	DataTable GetIdWorkScheduling(int year);

	DataTable GetDataShift(string code);

	DataTable GetTaskTemplateByCode(string code);

	DataTable GetTaskByWbsCode(string baseType, int baseEntry, int baseLine, string wbsCode);

	string CheckItemInDocument(int Id, int lineNum = -1);

	DataTable GetShifOrderByTime(string code);

	DataTable GetBrachByEmpID(int empId);

	string CheckMltpBrnchs();

	DataSet GetProjectByAbsEntryDMS(int absEntry);

	DataTable GetScheduleFromDate(string startDate);

	DataTable GetAllBaseLine(int absEntry);

	HttpResult SetApproval(string userName, NameID_Short model);

	DataTable GetTreeData(string itemCode);

	DataTable LoadDataDMS(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName, string filterString);

	HttpResult UndoApproval(int absEntry, string user);
}
