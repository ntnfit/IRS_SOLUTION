using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;

namespace Apzon.HttpClientHelper;

internal class UdfClient : IUdfClient
{
	private const string FormController = "UDF";

	public async Task<List<UserFieldsImpl>> GetListField(string tblName)
	{
		try
		{
			using HttpClientHelper<UserFieldsImpl> helper = new HttpClientHelper<UserFieldsImpl>(Configuration.ApiServer, "UDF");
			return await helper.GetListAsyncByPr("GetListField", new
			{
				tableName = tblName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<List<Common.Table_Code>> GetLinkedTable(string linkedTable, string userName, int branch)
	{
		try
		{
			using HttpClientHelper<Common.Table_Code> helper = new HttpClientHelper<Common.Table_Code>(Configuration.ApiServer, "UDF");
			return await helper.GetListAsync("GetLinkedTable", new { linkedTable, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> UpdateDataUdf(UserFieldsImpl udfField, QueryParamater keyCondition, string tableName)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UDF");
		return await helper.PostAsync("UpdateDataUdf", new Tuple<UserFieldsImpl, QueryParamater, string>(udfField, keyCondition, tableName));
	}

	public DataTable GetListKeyAlias(string tblName)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			return httpClientHelper.Get("GetListKeyAlias", new
			{
				tableName = tblName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public async Task<HttpResult> RefreshUDF()
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UDF");
		return await helper.PostAsync("RefreshUDF", new object());
	}
}
