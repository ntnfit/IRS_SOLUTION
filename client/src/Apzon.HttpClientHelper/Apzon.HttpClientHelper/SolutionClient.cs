using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class SolutionClient : ISolutionClient
{
	private const string FormController = "Solution";

	public async Task<DataTable> Search(int sltCode, string itemCode, int status)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution");
			return await hp.GetAsync("Search", new
			{
				sltCode = sltCode,
				status = status,
				itemCode = (itemCode ?? "")
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetLine(int atcEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution");
			return await http.GetAsync("GetLine", new { atcEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetSolutionStatuses()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution");
			return await http.GetAsync("GetSolutionStatuses");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetOwner()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution");
			return await http.GetAsync("GetOwner");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> FindDataNextLast(int id, int type)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution");
			return await hp.GetAsync("FindDataNextLast", new { id, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> Add(DataSet ds)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Solution");
			return await helper.PostAsync("Add", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> Update(DataSet ds)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Solution");
			return await helper.PostAsync("Update", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteDocs(int id)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Solution");
			return await helper.PostParamsAsJsonAsync("DeleteDocs", id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<int> GetMaxSltCode()
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Solution");
			return await helper.GetIntAsync("GetMaxSltCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
