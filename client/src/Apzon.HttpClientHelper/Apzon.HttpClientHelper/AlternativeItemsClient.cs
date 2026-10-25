using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;

namespace Apzon.HttpClientHelper;

public class AlternativeItemsClient : IAlternativeItemsClient
{
	private const string controller = "AlternativeItems";

	public async Task<DataTable> GetItemNameByItemCode(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AlternativeItems");
			return await helper.GetAsync("GetItemNameByItemCode", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataDoc(Tuple<string, Enums.NavigateType> tuple)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AlternativeItems");
			return await helper.PostAsync("GetDataDoc", tuple);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetData(string originItem)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AlternativeItems");
			return await helper.GetAsync("GetData", new { originItem });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> Save(Tuple<string, DataTable> tuple)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "AlternativeItems");
			return await helper.PostAsync("Save", tuple);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteData(string itemNo)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "AlternativeItems");
			return await helper.PostAsync("DeleteData", itemNo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetDataCfl(string itemCode, string userName, int branch, string cardCode, DateTime docDate)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "AlternativeItems");
		return await helper.GetAsync("GetDataCfl", new
		{
			itemCode = itemCode,
			userName = userName,
			branch = branch,
			cardCode = cardCode,
			docDate = docDate.ToString("yyyyMMdd")
		});
	}
}
