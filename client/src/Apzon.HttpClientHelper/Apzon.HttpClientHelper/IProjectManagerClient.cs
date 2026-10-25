using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IProjectManagerClient
{
	Task<DataTable> LoadData(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName);

	Task<DataSet> GetDataById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<DataTable> LoadSubProjectByMasterId(int projectId);

	Task<DataTable> GetDataBOMSaleOrder(string code);

	Task<DataTable> GetDataItemProduction(string code);

	Task<DataTable> GetDataItemInBom(string code);

	Task<DataTable> GetDataItemGroup();

	Task<HttpResult> UpdateStatus(int docEntry, string status);

	Task<DataTable> FindNextLastFrist(int id, int type);

	Task<DataSet> GetDataLineByWbsTemplate(string code);

	DataTable GetIdWorkScheduling(int year);

	DataTable GetDataShift(string code);

	Task<DataTable> GetTaskTemplateByCode(string code);

	Task<DataTable> GetTaskByWbsCode(string baseType, int baseEntry, int baseLine, string wbsCode);

	Task<DataTable> GetStruct();

	string CheckItemInDocument(int Id, int lineNum = -1);

	DataTable GetShifOrderByTime(string code);

	DataTable GetBrachByEmpID(int empId);

	string CheckMltpBrnchs();

	Task<DataTable> GetScheduleFromDate(string startDate);

	Task<DataTable> GetAllBaseLine(int absEntry);

	Task<HttpResult> SetApproval(string status, int id);

	Task<DataTable> GetTreeData(string itemCode);

	Task<HttpResult> UndoApproval(int absEntry);
}
