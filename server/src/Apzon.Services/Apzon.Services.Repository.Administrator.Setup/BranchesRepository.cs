using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class BranchesRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IBranches, IService<DataTable>
{
	private string TempTableTab = "#dtOBPL";

	public string Insert(DataTable entity)
	{
		throw new NotImplementedException();
	}

	public string Update(DataTable entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(DataTable entity)
	{
		throw new NotImplementedException();
	}

	public DataTable GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OBPL ORDER BY \"BPLId\";", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dtDoc)
	{
		return Process(ApzObjectTypes.oBranches, -1, Enums.TransactionType.U, dtDoc);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("DELETE dbo.OBPL\r\n                                     INSERT INTO dbo.OBPL\r\n                                     SELECT * FROM #dtOBPL", CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			foreach (DataRow row in document.Rows)
			{
				ExecuteNonQuery("UPDATE dbo.OWHS SET BPLid = @id WHERE WhsCode = @whs or WhsCode = @whsRes", CommandType.Text, new SqlParameter("@id", row["BPLId"]), new SqlParameter("@whs", row["DflWhs"]), new SqlParameter("@whsRes", row["DfltResWhs"]));
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetMaxCode()
	{
		try
		{
			string query = "SELECT MAX(\"BPLId\") FROM OBPL; ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Routing  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistBPLInOWHS(Tuple<string, string> tup)
	{
		try
		{
			return ExecuteDataTable("SELECT \"BPLid\" FROM OWHS WHERE \"WhsCode\"=@whs OR  \"WhsCode\"=@resWhs;", CommandType.Text, new SqlParameter("@whs", tup.Item1), new SqlParameter("@resWhs", tup.Item2));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
