using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Financials;

namespace Apzon.Services.Repository.Financials;

public class CurrenciesRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ICurrencies
{
	private DataTable _dt = new DataTable();

	private const string TempTableTab = "#dtOCRN";

	public DataTable GetData()
	{
		try
		{
			_dt = ExecuteDataTable("select *, 'Y' \"IsExist\" from OCRN;", CommandType.Text, (SqlParameter[])null);
			if (_dt != null && _dt.Rows.Count > 0)
			{
				return _dt;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dtObj)
	{
		return Process(ApzObjectTypes.oBranches, -1, Enums.TransactionType.U, dtObj);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			DataTable tableStruct = GetTableStruct("OCRN");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			document = document.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(document, "#dtOCRN");
			BulkCopy(document, "#dtOCRN");
			DataTable dataTable = ExecuteDataTable("APZ_Currencies_Save", CommandType.StoredProcedure);
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				return new HttpResult(Enums.MessageCode.Error, $"Currency exists in table {Function.ToString(dataTable.Rows[0][0])}");
			}
			objId = Function.ToString(dataTable);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string ExistInDocument(string curr)
	{
		string result = "";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@CurrCode", curr)
			};
			DataTable dataTable = ExecuteDataTable("APZ_Currency_ExistInDocument", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public HttpResult Remove(string currCode)
	{
		try
		{
			string query = "delete from OCRG where \"CurrCode\"=@currCode;";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@currCode", currCode)) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Delete Failed");
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
