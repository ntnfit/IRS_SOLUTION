using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class RetailChooseFromListClient : IRetailChooseFromList
{
	private const string FormController = "RetailChooseFromList";

	public async Task<DataSet> ListItems(string username, string storecode, string itemcode, string cardcode, string barcode)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "RetailChooseFromList");
			return await helper.GetAsync("ListItems", new { username, storecode, itemcode, cardcode, barcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> Customer(string username, string storecode, string cardcode)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "RetailChooseFromList");
			return await helper.GetAsync("Customer", new { username, storecode, cardcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
