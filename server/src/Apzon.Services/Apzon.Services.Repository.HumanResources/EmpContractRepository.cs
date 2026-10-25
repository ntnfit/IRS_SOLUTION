using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class EmpContractRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IEmpContract
{
	private string TempTableTab = "#dtAPZ_OCTY";

	private string TempTableContract = "#dtAPZ_OHEC";

	public DataTable LoadDoc(int empID)
	{
		try
		{
			string query = "SELECT a.*, b.\"empID\", b.\"firstName\", b.\"lastName\", b.\"middleName\", c.\"code\" AS \"typeName\" FROM APZ_OHEC a INNER JOIN OHEM b ON a.\"empID\" = b.\"empID\" LEFT OUTER JOIN APZ_OCTY c ON a.\"type\" = c.\"typeID\" WHERE b.\"empID\" = @empID;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@empID", empID));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckContractType(int typeID)
	{
		try
		{
			string query = "SELECT * FROM APZ_OHEC WHERE \"type\"=@typeID;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@typeID", typeID));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadContractTypes()
	{
		try
		{
			string query = "select * from APZ_OCTY;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveContract(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@contractID", Function.ParseInt(dt.Rows[0]["contractID"])));
				list.Add(new SqlParameter("@empID", Function.ParseInt(dt.Rows[0]["empID"])));
				list.Add(new SqlParameter("@code", Function.ToString(dt.Rows[0]["code"])));
				list.Add(new SqlParameter("@type", Function.ParseInt(dt.Rows[0]["type"])));
				list.Add(new SqlParameter("@signerID", Function.ToString(dt.Rows[0]["signerID"])));
				list.Add(new SqlParameter("@signer", Function.ToString(dt.Rows[0]["signer"])));
				DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["signDate"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					list.Add(new SqlParameter("@signDate", dateTime.Value));
				}
				else
				{
					list.Add(new SqlParameter("@signDate", DBNull.Value));
				}
				dateTime = Function.ParseDateTime(dt.Rows[0]["startDate"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					list.Add(new SqlParameter("@startDate", dateTime.Value));
				}
				else
				{
					list.Add(new SqlParameter("@startDate", DBNull.Value));
				}
				dateTime = Function.ParseDateTime(dt.Rows[0]["endDate"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					list.Add(new SqlParameter("@endDate", dateTime.Value));
				}
				else
				{
					list.Add(new SqlParameter("@endDate", DBNull.Value));
				}
				list.Add(new SqlParameter("@ProbSalary", Function.ParseDouble(dt.Rows[0]["ProbSalary"])));
				list.Add(new SqlParameter("@basicSalary", Function.ParseDouble(dt.Rows[0]["basicSalary"])));
				list.Add(new SqlParameter("@salary", Function.ParseDouble(dt.Rows[0]["salary"])));
				list.Add(new SqlParameter("@salaryCurr", Function.ToString(dt.Rows[0]["salaryCurr"])));
				list.Add(new SqlParameter("@PymMeth", Function.ToString(dt.Rows[0]["PymMeth"])));
				list.Add(new SqlParameter("@note", Function.ToString(dt.Rows[0]["note"])));
				list.Add(new SqlParameter("@BPLId", Function.ParseInt(dt.Rows[0]["BPLId"])));
				if (ExecuteNonQuery("APZ_EmpContract_SaveData", CommandType.StoredProcedure, list.ToArray()) <= 0)
				{
					return "Error!";
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteContract(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@contractID", Function.ParseInt(dt.Rows[0]["contractID"])));
				list.Add(new SqlParameter("@empID", Function.ParseInt(dt.Rows[0]["empID"])));
				if (ExecuteNonQuery("DELETE FROM APZ_OHEC WHERE contractID=@contractID AND empID=@empID", CommandType.Text, list.ToArray()) <= 0)
				{
					return "Error!";
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public HttpResult SaveData(DataTable dtDoc, Enums.TransactionType transtype = Enums.TransactionType.A)
	{
		return Process(ApzObjectTypes.oContractTypes, -1, transtype, dtDoc);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		if (document.IsNotNull())
		{
			if (document.Columns.Contains("empID"))
			{
				CreateTempTable(document, TempTableContract);
				BulkCopy(document, TempTableContract);
				object obj = ExecuteScalar("APZ_EmpContract_Save", CommandType.StoredProcedure);
				objId = Function.ToString(obj);
				objNum = objId;
			}
			else
			{
				CreateTempTable(document, TempTableTab);
				BulkCopy(document, TempTableTab);
				object obj2 = ExecuteScalar(SaveDataGetQuery(), CommandType.Text);
				objId = Function.ToString(obj2);
				objNum = objId;
			}
		}
		return new HttpResult();
	}

	protected virtual string SaveDataGetQuery()
	{
		return "\r\ndeclare @i int  = (SELECT ISNULL(MAX(typeID),0) FROM APZ_OCTY)  \r\n\tUPDATE #dtAPZ_OCTY\r\n\tSET typeID  = @i , @i = @i + 1\r\n\twhere typeID = 0 OR typeID is null\r\n\r\n                                     DELETE dbo.APZ_OCTY\r\n                                     INSERT INTO dbo.APZ_OCTY\r\n                                     SELECT * FROM #dtAPZ_OCTY";
	}
}
