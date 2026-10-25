using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class TimeSheetingClient : ITimeSheetingClient
{
	private const string FormController = "TimeSheeting";

	public async Task<DataTable> LoadWorkType()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetDataTableAsync("GetWorkType");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadOvertimeLeave(string period, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetDataTableAsync("LoadOvertimeLeave", new { period, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadOvertimeNew(string period, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetDataTableAsync("LoadOvertimeNew", new { period, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveOvertimeInformation(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> _hel = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheeting");
			return await _hel.PostAsync("SaveOvertimeInformation", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> CancelTimeSheeting(string period)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("CancelTimeSheeting", new { period });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadWorkCode(int type)
	{
		using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
		return await _help.GetAsync("GetWorkCode", new { type });
	}

	public async Task<string> SaveWorkCode(DataTable dt)
	{
		using HttpClientHelper<string> _hel = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
		return await _hel.PostAsync("SaveWorkCode", dt);
	}

	public async Task<string> RemoveWorkCode(string id, int branch)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
		return await hp.GetAsync("RemoveWorkCode", new { id, branch });
	}

	public async Task<string> CheckWorkCode(string id, int branch)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
		return await hp.GetAsync("CheckWorkCode", new { id, branch });
	}

	public async Task<DataTable> GetOvertimeType()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("GetOvertimeType");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveOvertimeType(DataTable dt)
	{
		using HttpClientHelper<HttpResult> _hel = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheeting");
		return await _hel.PostAsync("SaveOvertimeType", dt);
	}

	public async Task<string> RemoveOvertimeType(string code, int branch)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
		return await hp.GetAsync("RemoveOvertimeType", new { code, branch });
	}

	public async Task<string> CheckOvertimeType(string code)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
		return await hp.PostAsync("CheckOvertimeType", code);
	}

	public async Task<HttpResult> SaveBiometricData(DataSet ds)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheeting");
		return await helper.PostAsync("SaveBiometricData", ds);
	}

	public async Task<HttpResult> SaveTimeSheetInOut(DataSet ds)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheeting");
		return await helper.PostAsync("SaveTimeSheetInOut", ds);
	}

	public async Task<HttpResult> SaveBiometricMapping(DataTable dt)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheeting");
		return await helper.PostAsync("SaveBiometricMapping", dt);
	}

	public async Task<HttpResult> SaveBiometricConfig(DataTable dt)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheeting");
		return await helper.PostAsync("SaveBiometricConfig", dt);
	}

	public async Task<DataTable> LoadTimeSheetInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetDataTableAsync("LoadTimeSheetInOut", new { branch, dept, empID, empName, status, period, workcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> LoadInOutByBaseDoc(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("LoadInOutByBaseDoc", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadTimeSheetFromInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetDataTableAsync("LoadTimeSheetFromInOut", new { branch, dept, empID, empName, status, period, workcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadClockReport(int branch, string dept, int empID, string empName, string status, string period)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetDataTableAsync("LoadClockReport", new { branch, dept, empID, empName, status, period });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadClockReportByEntry(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetDataTableAsync("LoadClockReportByEntry", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadBiometricMapping(DataTable dt)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.PostAsync("LoadBiometricMapping", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> LoadWorkCodeForEmpByDate(int inTime, int outTime, string period, int empId, string WorkDate)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("LoadWorkCodeForEmpByDate", new { inTime, outTime, period, empId, WorkDate });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadImportList()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("LoadImportList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadInOutList()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("LoadInOutList");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> LoadInOutByAbsEntry(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("LoadInOutByAbsEntry", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetAllBiometricConfig()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("GetAllBiometricConfig");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> RemoveConfig(int absEntry)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("RemoveConfig", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> RemoveBiometricData(int absEntry)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("RemoveBiometricData", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> RemoveTimeSheetInOut(int absEntry)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("RemoveTimeSheetInOut", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSetResult> GetDataSetting()
	{
		using HttpClientHelper<DataSetResult> _help = new HttpClientHelper<DataSetResult>(Configuration.ApiServer, "TimeSheeting");
		return await _help.GetAsync("GetDataSetting");
	}

	public async Task<DataTable> GetOvertimeTypeByBaseDoc(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> _hel = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _hel.GetAsync("GetOvertimeTypeByBaseDoc", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
