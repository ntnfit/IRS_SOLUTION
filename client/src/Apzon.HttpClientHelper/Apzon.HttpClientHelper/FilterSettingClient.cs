using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Global;

namespace Apzon.HttpClientHelper;

internal class FilterSettingClient : IFilterSettingClient
{
	public string FormController = "FilterSetting";

	public async Task<DataSet> GetCategory()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetCategory");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetFilter(string category)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetFilter", new { category });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetFilterByCode(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetFilterByCode", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataFilter(string datafilter)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("GetDataFilter", new Tuple<string, string>(datafilter, GlobalData.UserCodeLogin));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Save(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("SaveData", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataSet> GetUsers()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Users");
			return await helper.GetAsync("GetAll");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetUserByFilter(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetUserByFilter", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> SaveUserSign(DataTable dt)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.PostAsJsonAsync("SaveUserSign", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public async Task<string> Remove(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.PostParamsAsJsonAsync("RemoveData", code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public async Task<DataTable> GetFilterSelect(string userName, string objType)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetFilterSelect", new { userName, objType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task InsertQueryFilter(string username)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		await helper.PostAsync("InsertQueryFilter", new { username });
	}
}
