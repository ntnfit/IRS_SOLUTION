using System;
using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface ITimeSheeting
{
	DataTable LoadTimeSheeting(int branch, string dept, int empID, string empName, string status, string period, string workcode, int baseEntry);

	DataTable LoadTimeSheetingSetting(string userSign);

	DataSet LoadTimeSheetingCopyFrom(int baseEntry);

	DataTable ExportTimeSheeting(int branch, string dept, int empID, string empName, string status, string period);

	HttpResult SaveTimeSheeting(DataTable dt);

	string SaveTimeSheetingSetting(DataTable dt);

	string SaveImportTimeSheeting(DataSet dt);

	DataTable LoadImportTimeSheetingTemplate(string tableName, string userSign);

	DataTable LoadOvertimeLeave(string period, int branch);

	DataTable LoadOvertimeNew(string period, int branch);

	HttpResult SaveOvertimeInformation(DataTable dt);

	string SaveImportTimeSheetingTemplate(DataTable dt);

	string CancelTimeSheeting(string absEntry);

	DataTable GetWorkType();

	string SaveWorkType(DataTable dt);

	string RemoveWorkType(int Id);

	DataTable GetWorkCode(int type);

	string SaveWorkCode(DataTable dt);

	string RemoveWorkCode(string id, int branch);

	string CheckWorkCode(string id, int branch);

	DataTable GetOvertimeType();

	HttpResult SaveOvertimeType(DataTable dt);

	string RemoveOvertimeType(string code, int branch);

	string CheckOvertimeType(string code);

	HttpResult SaveBiometricData(DataSet ds);

	HttpResult SaveTimeSheetInOut(DataSet ds);

	HttpResult SaveBiometricMapping(DataTable dt);

	DataTable LoadBiometricMapping(DataTable dt);

	DataTable LoadTimeSheetInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode);

	DataSet LoadInOutByBaseDoc(int absEntry);

	DataTable LoadTimeSheetingFromInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode);

	DataTable LoadClockReport(int branch, string dept, int empID, string empName, string status, string period);

	DataTable LoadClockReportByEntry(int absEntry);

	DataTable LoadImportList();

	DataTable LoadInOutList();

	DataSet LoadInOutByAbsEntry(int absEntry);

	DataTable GetAllBiometricConfig();

	HttpResult SaveBiometricConfig(DataTable dt);

	string RemoveConfig(int absEntry);

	string RemoveBiometricData(int absEntry);

	string RemoveTimeSheetInOut(int absEntry);

	DataSetResult GetDataSetting();

	DataTable GetOvertimeTypeByBaseDoc(int absEntry);

	DataSet LoadWorkCodeForEmpByDate(int inTime, int outTime, string period, int empId, DateTime WorkDate);
}
