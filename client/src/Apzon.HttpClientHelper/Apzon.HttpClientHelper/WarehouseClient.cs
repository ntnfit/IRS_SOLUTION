using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

internal class WarehouseClient : IWarehouseClient
{
	private const string FormController = "Warehouse";

	public async Task<DataTable> GetAllCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Warehouse");
			return await helper.GetAsync("GetAllCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetRecord(string position, string whsCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Warehouse");
			return await helper.GetAsync("GetRecord", new { position, whsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteWarehouse(string whsCode)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Warehouse");
			return await helper.PostAsync("DeleteWarehouse", whsCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public Common.Table_Code WhsCheckFollowBin(string whsCode)
	{
		using HttpClientHelper<Common.Table_Code> httpClientHelper = new HttpClientHelper<Common.Table_Code>(Configuration.ApiServer, "Warehouse");
		return httpClientHelper.Get("WhsCheckFollowBin", new { whsCode });
	}

	public Common.Table_Code WhsGetDefBinItem(string whsCode, string itemCode)
	{
		using HttpClientHelper<Common.Table_Code> httpClientHelper = new HttpClientHelper<Common.Table_Code>(Configuration.ApiServer, "Warehouse");
		return httpClientHelper.Get("WhsGetDefBinItem", new { whsCode, itemCode });
	}

	public async Task<string> Save(WarehouseImpl doc)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Warehouse");
		return await helper.PostAsync("Save", doc);
	}
}
