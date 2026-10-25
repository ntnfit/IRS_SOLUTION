using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.HumanResources.TimeSheeting;

public class TimeSheetingController : BaseApiController
{
	[HttpGet]
	public DataTable LoadTimeSheeting(int branch, string dept, int empID, string empName, string status, string period, string workcode, int baseEntry)
	{
		return UnitOfWork.TimeSheeting.LoadTimeSheeting(branch, dept, empID, empName, status, period, workcode, baseEntry);
	}

	[HttpGet]
	public DataTable LoadTimeSheetingSetting(string userSign)
	{
		return UnitOfWork.TimeSheeting.LoadTimeSheetingSetting(userSign);
	}

	[HttpGet]
	public DataSet LoadTimeSheetingCopyFrom(int baseEntry)
	{
		return UnitOfWork.TimeSheeting.LoadTimeSheetingCopyFrom(baseEntry);
	}

	[HttpGet]
	public DataTable ExportTimeSheeting(int branch, string dept, int empID, string empName, string status, string period)
	{
		return UnitOfWork.TimeSheeting.ExportTimeSheeting(branch, dept, empID, empName, status, period);
	}

	[HttpPost]
	public HttpResult SaveTimeSheeting([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveTimeSheeting(dt);
	}

	[HttpPost]
	public string SaveTimeSheetingSetting([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveTimeSheetingSetting(dt);
	}

	[HttpPost]
	public string SaveImportTimeSheeting([FromBody] DataSet dt)
	{
		return UnitOfWork.TimeSheeting.SaveImportTimeSheeting(dt);
	}

	[HttpGet]
	public DataTable LoadImportTimeSheetingTemplate(string tableName, string userSign)
	{
		return UnitOfWork.TimeSheeting.LoadImportTimeSheetingTemplate(tableName, userSign);
	}

	[HttpGet]
	public DataTable LoadOvertimeLeave(string period, int branch)
	{
		return UnitOfWork.TimeSheeting.LoadOvertimeLeave(period, branch);
	}

	[HttpGet]
	public DataTable LoadOvertimeNew(string period, int branch)
	{
		return UnitOfWork.TimeSheeting.LoadOvertimeNew(period, branch);
	}

	[HttpPost]
	public HttpResult SaveOvertimeInformation([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveOvertimeInformation(dt);
	}

	[HttpPost]
	public string SaveImportTimeSheetingTemplate([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveImportTimeSheetingTemplate(dt);
	}

	[HttpGet]
	public string CancelTimeSheeting(string period)
	{
		return UnitOfWork.TimeSheeting.CancelTimeSheeting(period);
	}

	[HttpGet]
	public DataTable GetWorkType()
	{
		return UnitOfWork.TimeSheeting.GetWorkType();
	}

	[HttpPost]
	public string RemoveWorkType([FromBody] int Id)
	{
		return UnitOfWork.TimeSheeting.RemoveWorkType(Id);
	}

	[HttpPost]
	public string SaveWorkType([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveWorkType(dt);
	}

	[HttpGet]
	public DataTable GetWorkCode(int type)
	{
		return UnitOfWork.TimeSheeting.GetWorkCode(type);
	}

	[HttpGet]
	public string RemoveWorkCode(string id, int branch)
	{
		return UnitOfWork.TimeSheeting.RemoveWorkCode(id, branch);
	}

	[HttpPost]
	public string SaveWorkCode([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveWorkCode(dt);
	}

	[HttpGet]
	public string CheckWorkCode(string id, int branch)
	{
		return UnitOfWork.TimeSheeting.CheckWorkCode(id, branch);
	}

	[HttpGet]
	public DataTable GetOvertimeType()
	{
		return UnitOfWork.TimeSheeting.GetOvertimeType();
	}

	[HttpGet]
	public string RemoveOvertimeType(string code, int branch)
	{
		return UnitOfWork.TimeSheeting.RemoveOvertimeType(code, branch);
	}

	[HttpPost]
	public HttpResult SaveOvertimeType([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveOvertimeType(dt);
	}

	public string CheckOvertimeType([FromBody] string code)
	{
		return UnitOfWork.TimeSheeting.CheckOvertimeType(code);
	}

	[HttpPost]
	public HttpResult SaveBiometricData([FromBody] DataSet ds)
	{
		return UnitOfWork.TimeSheeting.SaveBiometricData(ds);
	}

	[HttpPost]
	public DataTable LoadBiometricMapping([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.LoadBiometricMapping(dt);
	}

	[HttpPost]
	public HttpResult SaveTimeSheetInOut([FromBody] DataSet ds)
	{
		return UnitOfWork.TimeSheeting.SaveTimeSheetInOut(ds);
	}

	[HttpPost]
	public HttpResult SaveBiometricMapping([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveBiometricMapping(dt);
	}

	[HttpPost]
	public HttpResult SaveBiometricConfig([FromBody] DataTable dt)
	{
		return UnitOfWork.TimeSheeting.SaveBiometricConfig(dt);
	}

	[HttpGet]
	public DataTable LoadTimeSheetInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode)
	{
		return UnitOfWork.TimeSheeting.LoadTimeSheetInOut(branch, dept, empID, empName, status, period, workcode);
	}

	[HttpGet]
	public DataSet LoadInOutByBaseDoc(int absEntry)
	{
		return UnitOfWork.TimeSheeting.LoadInOutByBaseDoc(absEntry);
	}

	[HttpGet]
	public DataTable LoadTimeSheetFromInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode)
	{
		return UnitOfWork.TimeSheeting.LoadTimeSheetingFromInOut(branch, dept, empID, empName, status, period, workcode);
	}

	[HttpGet]
	public DataTable LoadClockReport(int branch, string dept, int empID, string empName, string status, string period)
	{
		return UnitOfWork.TimeSheeting.LoadClockReport(branch, dept, empID, empName, status, period);
	}

	[HttpGet]
	public DataTable LoadClockReportByEntry(int absEntry)
	{
		return UnitOfWork.TimeSheeting.LoadClockReportByEntry(absEntry);
	}

	[HttpGet]
	public DataSet LoadInOutByAbsEntry(int absEntry)
	{
		return UnitOfWork.TimeSheeting.LoadInOutByAbsEntry(absEntry);
	}

	[HttpGet]
	public DataTable LoadImportList()
	{
		return UnitOfWork.TimeSheeting.LoadImportList();
	}

	[HttpGet]
	public DataTable LoadInOutList()
	{
		return UnitOfWork.TimeSheeting.LoadInOutList();
	}

	[HttpGet]
	public DataTable GetAllBiometricConfig()
	{
		return UnitOfWork.TimeSheeting.GetAllBiometricConfig();
	}

	[HttpGet]
	public string RemoveConfig(int absEntry)
	{
		return UnitOfWork.TimeSheeting.RemoveConfig(absEntry);
	}

	[HttpGet]
	public string RemoveBiometricData(int absEntry)
	{
		return UnitOfWork.TimeSheeting.RemoveBiometricData(absEntry);
	}

	[HttpGet]
	public string RemoveTimeSheetInOut(int absEntry)
	{
		return UnitOfWork.TimeSheeting.RemoveTimeSheetInOut(absEntry);
	}

	[HttpGet]
	public DataSetResult GetDataSetting()
	{
		return UnitOfWork.TimeSheeting.GetDataSetting();
	}

	[HttpGet]
	public DataTable GetOvertimeTypeByBaseDoc(int absEntry)
	{
		return UnitOfWork.TimeSheeting.GetOvertimeTypeByBaseDoc(absEntry);
	}

	[HttpGet]
	public DataSet LoadWorkCodeForEmpByDate(int inTime, int outTime, string period, int empId, string WorkDate)
	{
		return UnitOfWork.TimeSheeting.LoadWorkCodeForEmpByDate(inTime, outTime, period, empId, Function.ParseDateTimes(WorkDate));
	}
}
