using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class LocationRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ILocation
{
	private string TempTableTab = "#dtOLCT";

	public List<LocationImpl> Cfl()
	{
		throw new NotImplementedException();
	}

	public DataTable GetLocation()
	{
		try
		{
			string query = "select \"Code\", \"Location\"  from OLCT;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLocation ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable ShowLocation()
	{
		try
		{
			string query = "select \"Code\",\"Location\" from OLCT;   ";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowLocation ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable Search(LocationImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"Code\", \"Location\"  from OLCT\r\n                                Where 1=1 AND \"Code\" = @code; ";
			result = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", doc.Code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Location Search - Doc ", ex.Message);
		}
		return result;
	}

	public string SaveDoc(LocationImpl doc)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				bool flag = Check(doc.Code);
				string empty2 = string.Empty;
				if (!flag)
				{
					empty2 = "INSERT INTO OLCT  (Code , Location)\r\n                                                     VALUES \r\n                                        ( ";
					empty2 += " @code ";
					list.Add(new SqlParameter("@code", doc.Code));
					empty2 += ", @location ";
					list.Add(new SqlParameter("@location", doc.Location));
					empty2 += " )";
				}
				else
				{
					list = new List<SqlParameter>();
					empty2 = "UPDATE OLCT  \r\n                            SET ";
					empty2 += "Location= @location ";
					list.Add(new SqlParameter("@location", doc.Location));
					empty2 += " WHERE Code =@code";
					list.Add(new SqlParameter("@code", doc.Code));
				}
				if (ExecuteNonQuery(empty2, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
				return "ERROR";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
				empty = ex.Message;
			}
			return empty;
		}
	}

	public string DeleteDoc(int code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = " delete from OLCT where \"Code\" = @code; \r\n                                 UPDATE ONNM SET \"AutoKey\" = (Select MAX(\"Code\") FROM OLCT) +1 WHERE \"ObjectCode\" = '144';";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@code", code)) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDoc  FAILED ", ex.Message);
			return null;
		}
	}

	public int GetMaxCode()
	{
		try
		{
			string maxCodeQuery = GetMaxCodeQuery();
			DataTable dataTable = ExecuteDataTable(maxCodeQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["Code"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxCode  FAILED ", ex.Message);
			return 0;
		}
	}

	protected virtual string GetMaxCodeQuery()
	{
		return "SELECT ISNULL( MAX(Code ),0) as Code\r\n                            FROM OLCT ";
	}

	public bool Check(int code)
	{
		try
		{
			string query = "SELECT   A.\"Code\" \r\n                            FROM OLCT A \r\n                            WHERE A.\"Code\" = @code; ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check Location  FAILED ", ex.Message);
			return false;
		}
	}

	public DataTable LoadLocation()
	{
		try
		{
			return ExecuteDataTable("select * from OLCT; ", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadLocation ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistLocationCode(int locCode)
	{
		try
		{
			string query = "SELECT \"ItemCode\" FROM OITM  WHERE \"Location\"=@Location; ";
			SqlParameter sqlParameter = new SqlParameter("@Location", locCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistItemGroupCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveOLCT(DataTable dtOLCT)
	{
		return Process(ApzObjectTypes.oWarehouseLocations, -1, Enums.TransactionType.A, dtOLCT);
	}

	public string GetLocationNameByCode(int code)
	{
		try
		{
			return Function.ToString(ExecuteScalar("Select \"Name\" From OCLO where \"Code\" = @code;", CommandType.Text, new SqlParameter("@code", code)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("DELETE dbo.OLCT\r\n                                     INSERT INTO dbo.OLCT\r\n                                     SELECT * FROM #dtOLCT \r\n                                     UPDATE ONNM SET AutoKey = (Select MAX(Code) FROM OLCT) +1 WHERE ObjectCode = '144'", CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
