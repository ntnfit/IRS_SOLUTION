using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class MenuStructureClient : IMenuStructureClient
{
	private const string FormController = "MenuStructure";

	public async Task<DataSet> GetListMenu(string lngCode = "")
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "MenuStructure");
			return await helper.GetAsync("GetListMenu", new { lngCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> AddNewFolder(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "MenuStructure");
			return await helper.PostAsync("AddNewFolder", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> RelocateFolder(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "MenuStructure");
			return await helper.PostAsync("RelocateFolder", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> Rename(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "MenuStructure");
			return await helper.PostAsync("Rename", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteFolder(int id)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "MenuStructure");
			return await helper.PostAsync("DeleteFolder", id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetListCustomizeMenu()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MenuStructure");
			return await helper.GetAsync("GetListCustomizeMenu");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
