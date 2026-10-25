using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Global;

namespace Apzon.HttpClientHelper;

internal class QueriesClient : IQueriesClient
{
	private const string FormController = "Queries";

	public async Task<DataTable> GetParameters(string qname)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Queries");
		return await helper.GetAsync("GetParameters", new { qname });
	}

	public async Task<DataTable> GetColumns(string qName, CommandType cmdType, DataTable prs)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Queries");
		return await helper.PostAsync("GetColumns", new Tuple<string, CommandType, DataTable>(qName, cmdType, prs));
	}

	public async Task<HttpResult> SaveExtractQuery(DataSet ds)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Queries");
		return await helper.PostAsync("SaveExtractQuery", ds);
	}

	public async Task<DataSet> LoadParameters(int reportId)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Queries");
		return await helper.GetAsync("LoadParameters", new { reportId });
	}

	public async Task<DataTable> ListOfQueryReport()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Queries");
		return await helper.GetAsync("ListOfQueryReport", new
		{
			username = GlobalData.UserName
		});
	}

	public async Task<DataSet> SubmitQueryReport(DataTable prms)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Queries");
		return await helper.PostAsync("SubmitQueryReport", prms);
	}

	public async Task<string> RemoveExtractQR(string Id)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "Queries");
		return await helper.PostParamsAsJsonAsync("RemoveExtractQR", Id);
	}

	public async Task<DataSet> GetListQueries(string userName, string branch, string filter = "")
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Queries");
			return await helper.GetAsync("GetListQueries", new { userName, branch, filter });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public async Task<DataTable> GetQueryById(int queryId)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Queries");
			return await helper.GetAsync("GetQueryById", new { queryId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public async Task<string> GetQueryText(int queryId)
	{
		try
		{
			if (queryId != 0)
			{
				DataTable dataTable = await GetQueryById(queryId);
				if (dataTable.IsNotNull())
				{
					return Function.ToString(dataTable.Rows[0]["QString"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public async Task<DataTable> GetAuthGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Queries");
			return await helper.GetAsync("GetAuthGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
