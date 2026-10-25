using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class KPIsSettingClient : IKPIsSettingClient
{
	private const string FormController = "KPIsSetting";

	public async Task<DataSetResult> GetKPIsSetting()
	{
		try
		{
			using HttpClientHelper<Tuple<string, DataSet>> helper = new HttpClientHelper<Tuple<string, DataSet>>(Configuration.ApiServer, "KPIsSetting");
			Tuple<string, DataSet> tuple = await helper.GetAsync("GetKPIsSetting");
			return new DataSetResult(tuple.Item1, tuple.Item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public async Task<DataSetResult> GetDetail(int absEntry)
	{
		try
		{
			using HttpClientHelper<Tuple<string, DataSet>> helper = new HttpClientHelper<Tuple<string, DataSet>>(Configuration.ApiServer, "KPIsSetting");
			Tuple<string, DataSet> tuple = await helper.GetAsync("GetDetail", new { absEntry });
			return new DataSetResult(tuple.Item1, tuple.Item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public async Task<DataTable> LoadCopySetting(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "KPIsSetting");
			return await helper.GetAsync("LoadCopySetting", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public async Task<HttpResult> SaveCopySetting(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "KPIsSetting");
			return await http.PostAsync("SaveCopySetting", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> Add(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "KPIsSetting");
			return await http.PostAsync("Add", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> Update(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "KPIsSetting");
			return await http.PostAsync("Update", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> Delete(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "KPIsSetting");
			return await http.PostAsync("Delete", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
