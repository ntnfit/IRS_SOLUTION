using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class StoreRepository : BaseService<DataSet>, IStore
{
	public StoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string GetGroupUser(string clgCode)
	{
		try
		{
			string query = "select U_NAME from OUSR where USERID = @userId";
			if (clgCode.Length == 1)
			{
				DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@userId", Function.ParseInt(clgCode)));
				if (dataTable.IsNotNull())
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
			}
			string[] array = clgCode.Split(';');
			string text = string.Empty;
			string[] array2 = array;
			foreach (string obj in array2)
			{
				DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@userId", Function.ParseInt(obj)));
				if (dataTable2.IsNotNull())
				{
					text += Function.ToString(dataTable2.Rows[0][0]?.ToString() + ";");
				}
			}
			return text.Remove(text.Length - 1, 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public DataTable GetStore(string storeCode, string cardCode, string status)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@storeCode", Function.ToString(storeCode)));
			list.Add(new SqlParameter("@cardCode", Function.ToString(cardCode)));
			list.Add(new SqlParameter("@status", Function.ToString(status)));
			return ExecuteDataTable("APZ_Store_GetData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetEvents(int clgCode)
	{
		try
		{
			string query = "select b.Code, a.ClgCode, a.Details as Name, a.CardCode, a.Recontact, a.endDate, a.AllDayEvent, a.GroupUser, '' as Invite, b.Remarks \r\n                                 from APZ_OCLG a\r\n                                 left join APZ_D_STREV b on a.ClgCode = b.ClgCode\r\n                                 where a.ClgCode = @clgCode";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@clgCode", clgCode));
			if (dataTable.IsNotNull())
			{
				dataTable.Rows[0]["Invite"] = GetGroupUser(Function.ToString(dataTable.Rows[0]["GroupUser"]));
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStoreEvents(string storeCode)
	{
		try
		{
			string query = "select a.Code, a.ClgCode, b.Details as Name, b.CardCode, b.Recontact, b.endDate, b.AllDayEvent, b.GroupUser, '' as Invite, a.Remarks \r\n                                 from APZ_D_STREV a\r\n                                 inner join APZ_OCLG b on a.ClgCode = b.ClgCode \r\n                                 where ISNULL(a.Code, '') = @storeCode order by Name";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@storeCode", Function.ToString(storeCode)));
			if (dataTable.IsNotNull())
			{
				foreach (DataRow row in dataTable.Rows)
				{
					row["Invite"] = GetGroupUser(Function.ToString(row["GroupUser"]));
				}
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetGeography()
	{
		try
		{
			string query = "select b.Code, b.Name, a.CountryCode, a.CountryName from APZ_D_OGGP a\r\n                                 inner join APZ_D_GGP1 b on a.CountryCode = b.CountryCode\r\n                                 where a.Status = 'A' and b.Status = 'A'";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRecord(string position, string storeCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@position", position));
			list.Add(new SqlParameter("@storeCode", storeCode));
			return ExecuteDataTable("APZ_Store_GetRecord", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetLastEvent(string storeCode)
	{
		try
		{
			string query = "select a.Code, a.ClgCode, b.Details as Name, b.CardCode, b.Recontact, b.endDate, b.AllDayEvent, b.GroupUser, '' as Invite, a.Remarks \r\n                                 from APZ_D_STREV a\r\n                                 inner join APZ_OCLG b on a.ClgCode = b.ClgCode \r\n                                 where ISNULL(a.Code, '') = @storeCode and b.ClgCode = (select MAX(c.ClgCode) from APZ_OCLG c inner join APZ_D_STREV d on c.ClgCode = d.ClgCode where ISNULL(d.Code, '') = @storeCode)\r\n\t\t\t\t\t\t\t\t order by Name";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@storeCode", Function.ToString(storeCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(Tuple<DataSet, string> tuple)
	{
		if (tuple.Item2 == "A")
		{
			return Process(ApzObjectTypes.oStore, -1, Enums.TransactionType.A, tuple.Item1);
		}
		return Process(ApzObjectTypes.oStore, -1, Enums.TransactionType.U, tuple.Item1);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(document.Tables[0], "#APZ_D_STORE");
			BulkCopy(document.Tables[0], "#APZ_D_STORE");
			CreateTempTable(document.Tables[1], "#APZ_D_STREV");
			BulkCopy(document.Tables[1], "#APZ_D_STREV");
			if (transtype == Enums.TransactionType.A)
			{
				DataTable dataTable = ExecuteDataTable("APZ_Process_Store", CommandType.StoredProcedure, new SqlParameter("@type", Function.ToString(transtype)));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][1]));
				}
			}
			else
			{
				DataTable dataTable2 = ExecuteDataTable("APZ_Process_Store", CommandType.StoredProcedure, new SqlParameter("@type", Function.ToString(transtype)));
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable2.Rows[0][1]));
				}
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult CheckRemove(string storeCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_Store_CheckRemove", CommandType.StoredProcedure, new SqlParameter("@code", storeCode));
			if (dataTable.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, 1470000158 + " - " + Function.ToString(dataTable.Rows[0][1]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(Tuple<string> tuple)
	{
		try
		{
			if (ExecuteNonQuery("APZ_Store_Remove", CommandType.StoredProcedure, new SqlParameter("@storeCode", Function.ToString(tuple.Item1))) == -1)
			{
				return new HttpResult(Enums.MessageCode.Error, "Remove Failed");
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetParentsGeography(string ggpCode)
	{
		try
		{
			string query = "WITH CTE AS \r\n                               (\r\n                               \t   select Code, Name, CountryCode, FatherCode, 0 as Level, Id, FatherId\r\n                                   from APZ_D_GGP1\r\n                               \t   where Code = @ggpCode\r\n                                   union all\r\n                                   select t1.Code, t1.Name, t1.CountryCode, t1.FatherCode, cte.Level + 1, t1.Id, t1.FatherId\r\n                                   from APZ_D_GGP1 t1\r\n                                   inner join CTE cte ON cte.FatherId = t1.Id and cte.FatherCode = t1.Code\r\n                               )\r\n                               select * from CTE order by Level desc";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@ggpCode", Function.ToString(ggpCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
