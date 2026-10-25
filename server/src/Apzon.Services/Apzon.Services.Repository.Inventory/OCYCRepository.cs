using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class OCYCRepository : BaseService<OCYC_Impl>, IOCYC
{
	public OCYCRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetOCYC()
	{
		try
		{
			string query = " select isnull(Code,0)as Code, isnull(Name,'') as Name , ISNULL (Frequency,-1) as Frequency ,ISNULL(Day,-1)as Day\r\n                                 from OCYC order by Code ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOCYC  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable Search(string key)
	{
		try
		{
			string query = "select isnull(Code,0)as Code, isnull(Name,'') as Name , ISNULL (Frequency,-1) as Frequency ,ISNULL(Day,-1)as Day   WHERE Name like '%" + key + "%' order by Code  ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetList(string name, string frequency)
	{
		try
		{
			string text = "select isnull(Code,0)as Code, isnull(Name,'') as Name , ISNULL (Frequency,-1) as Frequency ,ISNULL(Day,-1)as Day   WHERE 1=1";
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " AND  Name like '%" + name + "%'   ";
			}
			if (!string.IsNullOrEmpty(frequency) && frequency != "0")
			{
				text = text + " AND  Frequency = '" + frequency + "'   ";
			}
			text += "  order by Code ";
			DataTable dataTable = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(Code ),0) as Code\r\n                            FROM OCYC ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["Code"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxCode  FAILED ", ex.ToString());
			return 0;
		}
	}

	public string Save(OCYC_Impl doc)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				bool flag = CheckDoc(doc.Code);
				string empty2 = string.Empty;
				if (!flag)
				{
					empty2 = "INSERT INTO OCYC  (Code , Name ,Frequency, Day, UserSign,Type)\r\n                               VALUES \r\n                                        ( ";
					empty2 += " @Code ";
					list.Add(new SqlParameter("@Code", Function.ParseInt(doc.Code)));
					empty2 += ", @Name ";
					list.Add(new SqlParameter("@Name", Function.ToString(doc.Name)));
					empty2 += ", @Frequency ";
					list.Add(new SqlParameter("@Frequency", Function.ToString(doc.Frequency)));
					empty2 += ", @Day ";
					list.Add(new SqlParameter("@Day", Function.ParseInt(doc.Day)));
					empty2 += ", @UserSign ";
					list.Add(new SqlParameter("@UserSign", Function.ParseDouble(doc.UserSign)));
					empty2 += ", @Type ";
					list.Add(new SqlParameter("@Type", Function.ToString(doc.Type)));
					empty2 += " )";
				}
				else
				{
					list = new List<SqlParameter>();
					empty2 = "UPDATE OCYC  \r\n                            SET\r\n                                Name= ";
					empty2 += " @Name ";
					list.Add(new SqlParameter("@Name", Function.ToString(doc.Name)));
					empty2 += ",Frequency= @Frequency ";
					list.Add(new SqlParameter("@Frequency", Function.ToString(doc.Frequency)));
					empty2 += ",Day= @Day ";
					list.Add(new SqlParameter("@Day", Function.ParseInt(doc.Day)));
					empty2 += ",UserSign= @UserSign ";
					list.Add(new SqlParameter("@UserSign", Function.ParseInt(doc.UserSign)));
					empty2 += " ,Type = @Type";
					list.Add(new SqlParameter("@Type", Function.ToString(doc.Type)));
					empty2 += " WHERE Code =@Code";
					list.Add(new SqlParameter("@Code", doc.Code));
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

	private bool CheckDoc(int p)
	{
		try
		{
			string query = "SELECT *\r\n                            FROM OCYC \r\n                            WHERE Code = '" + p + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}
}
