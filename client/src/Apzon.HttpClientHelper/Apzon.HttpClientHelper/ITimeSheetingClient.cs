using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ITimeSheetingClient
{
	Task<DataTable> LoadWorkType();

	Task<DataTable> LoadOvertimeLeave(string period, int branch);

	Task<DataTable> LoadOvertimeNew(string period, int branch);

	Task<HttpResult> SaveOvertimeInformation(DataTable dt);

	Task<DataTable> LoadWorkCode(int type = 1);

	Task<string> SaveWorkCode(DataTable dt);

	Task<string> RemoveWorkCode(string id, int branch);

	Task<string> CheckWorkCode(string id, int branch);

	Task<DataTable> GetOvertimeType();

	Task<HttpResult> SaveOvertimeType(DataTable dt);

	Task<string> RemoveOvertimeType(string code, int branch);

	Task<string> CheckOvertimeType(string code);

	Task<HttpResult> SaveBiometricData(DataSet ds);

	Task<HttpResult> SaveBiometricMapping(DataTable dt);

	Task<HttpResult> SaveTimeSheetInOut(DataSet ds);

	Task<HttpResult> SaveBiometricConfig(DataTable dt);

	Task<DataTable> LoadBiometricMapping(DataTable dt);

	Task<DataTable> LoadTimeSheetInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode);

	Task<DataSet> LoadInOutByBaseDoc(int absEntry);

	Task<DataTable> LoadClockReport(int branch, string dept, int empID, string empName, string status, string period);

	Task<DataTable> LoadTimeSheetFromInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode);

	Task<DataSet> LoadWorkCodeForEmpByDate(int inTime, int outTime, string period, int empId, string WorkDate);

	Task<DataTable> LoadClockReportByEntry(int absEntry);

	Task<DataSet> LoadInOutByAbsEntry(int absEntry);

	Task<DataTable> LoadImportList();

	Task<DataTable> LoadInOutList();

	Task<DataTable> GetAllBiometricConfig();

	Task<DataTable> GetOvertimeTypeByBaseDoc(int absEntry);

	Task<string> RemoveConfig(int absEntry);

	Task<string> RemoveBiometricData(int absEntry);

	Task<string> RemoveTimeSheetInOut(int absEntry);

	Task<string> CancelTimeSheeting(string period);

	Task<DataSetResult> GetDataSetting();
}
