using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Global;

namespace Apzon.HttpClientHelper;

internal class UdoClient : IUdoClient
{
	private const string FormController = "UDO";

	public async Task<int> GetMaxId(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
			return await helper.GetIntAsync("GetMaxId", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Save(DataSet ds)
	{
		try
		{
			if (ds.IsHasStructure() && ds.Tables[1].IsExistsColumn("U_UserSign2"))
			{
				ds.Tables[1].Rows[0]["U_UserSign2"] = GlobalData.UserName;
			}
			using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UDO");
			return await _help.PostAsync("Save", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataTable> GetUdoByCode(Common.Table_Code item)
	{
		DataTable dataTable = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
			dataTable = await helper.PostAsync("GetUdoByCode", item);
			if (dataTable == null)
			{
				return new DataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
		return dataTable;
	}

	public async Task<string> GetTypeByCode(string code)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "UDO");
			return await helper.GetAsync("GetTypeByCode", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			throw;
		}
	}

	public async Task<DataTable> GetStatus()
	{
		try
		{
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					{
						"Code",
						typeof(string)
					},
					{
						"Name",
						typeof(string)
					}
				}
			};
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "O";
			dataRow["Name"] = "Open";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "L";
			dataRow["Name"] = "Closed";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "C";
			dataRow["Name"] = "Canceled";
			dataTable.Rows.Add(dataRow);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetByDocEntry(Common.Table_Code item)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
			return await helper.PostAsync("GetDataByDocEntry", item);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetTableByName(string tblName)
	{
		new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
			return await helper.GetDataTableAsync("GetTableByName", new { tblName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetUdo1ByCode(string code)
	{
		new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
			return await helper.GetDataTableAsync("GetUdo1ByCode", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetListUdo()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
		return await helper.GetDataTableAsync("GetListUdo");
	}

	public async Task<DataTable> GetSeriesByUser(string udoCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
		return await helper.GetDataTableAsync("GetSeriesByUser", new { udoCode });
	}

	public async Task<string> GetNextNumber(string udoCode, int series)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "UDO");
		return await helper.GetAsync("GetNextNumber", new { udoCode, series });
	}
}
