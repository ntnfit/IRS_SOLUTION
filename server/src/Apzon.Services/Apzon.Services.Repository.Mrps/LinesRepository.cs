using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Mrps;

namespace Apzon.Services.Repository.Mrps;

public class LinesRepository : BaseService<DataSet>, ILines
{
	public LinesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetLines(DataRow r)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "Select * From APZ_OLIN";
			if (Function.ParseInt(r["BPLId"]) != -1)
			{
				text = text + " Where \"Branch\"=" + Function.ParseInt(r["BPLId"]);
			}
			text += ";";
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetALine(int id)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "Select * From APZ_LIN1 where \"LineID\"=" + id + ";Select * From APZ_LIN2 where \"LineID\"=" + id + ";";
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetUsers()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select USER_CODE \"Code\",U_NAME \"Name\" From OUSR;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable ExistsItemCodeInBranch(string ItemCode, int Branch, int LineID)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"ItemCode\" from APZ_LIN1 a where \"ItemCode\"=@ItemCode And a.\"LineID\"<>" + LineID + "and exists(select \"LineID\" From APZ_OLIN t where t.\"LineID\"=a.\"LineID\" and t.\"Branch\"=" + Branch + ");";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", ItemCode));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable ExistsLineCode(string LineCode, int LineID)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"LineCode\" from APZ_OLIN where \"LineCode\"=@LineCode And \"LineID\"<>" + LineID + ";";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@LineCode", LineCode));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable ExistsLine(int LineID)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"LineID\" from APZ_OLIN where \"LineID\"=" + LineID + ";";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string Save(DataSet ds)
	{
		try
		{
			return Process(ApzObjectTypes.oBusinessPartners, -1, Enums.TransactionType.A, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			string text = Function.ToString(ds.Tables[0].Rows[0]["LineType"]);
			if (ds.Tables.Count != 3 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0 || (ds.Tables[1] == null && text == "A") || (ds.Tables[1].Rows.Count == 0 && text == "A") || ds.Tables[2] == null || ds.Tables[2].Rows.Count == 0)
			{
				httpResult.Message = "Missing informations";
			}
			else
			{
				httpResult.Message = DeleteChildData(Function.ParseInt(ds.Tables[0].Rows[0]["LineID"]));
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					httpResult.Message = AddLine(ds.Tables[0].Rows[0], ds.Tables[1], ds.Tables[2], text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	private string AddLine(DataRow r, DataTable dt1, DataTable dt2, string lineType)
	{
		string text = "";
		try
		{
			int num = AddALine(r);
			if (num > 0)
			{
				for (int i = 0; i < dt1.Rows.Count; i++)
				{
					dt1.Rows[i]["LineID"] = num;
				}
				dt1.AcceptChanges();
				for (int j = 0; j < dt2.Rows.Count; j++)
				{
					dt2.Rows[j]["LineID"] = num;
				}
				dt2.AcceptChanges();
				if (lineType == "A")
				{
					text = SaveItems(dt1);
					if (string.IsNullOrEmpty(text))
					{
						text = SaveUsers(dt2);
					}
				}
				else
				{
					text = SaveUsers(dt2);
				}
			}
			else
			{
				text = "[LinesRepository].[AddLine]: No Rows updated.";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			text = ex.Message;
		}
		return text;
	}

	public string DeleteData(int LineID)
	{
		string result = "";
		try
		{
			string query = "apz_MRP_LINES_GetChildID";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@LineID", LineID));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			string text = "";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				text = Function.ToString(dataTable.Rows[0][0]);
				text = ((text[text.Length - 1] == ',') ? text.Remove(text.Length - 1) : text);
			}
			else
			{
				text = LineID.ToString();
			}
			query = "Delete FROM APZ_OLIN Where \"LineID\" in (" + GetChildLines(LineID) + ");\r\n                        Delete FROM APZ_LIN1 Where \"LineID\" in (" + text + ");\r\n                        Delete FROM APZ_LIN2 Where \"LineID\" in (" + text + ");";
			ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private string GetChildLines(int lineID)
	{
		try
		{
			string text = "";
			DataTable dataTable = ExecuteDataTable(GetChildLinesQuery(), CommandType.Text, new SqlParameter("@ID", lineID));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					text = text + Function.ToString(dataTable.Rows[i][0]) + ",";
				}
				if (text.EndsWith(","))
				{
					text = text.Remove(text.Length - 1);
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetChildLinesQuery()
	{
		return ";WITH ret AS(\r\n        SELECT  *\r\n        FROM    APZ_OLIN\r\n        WHERE   LineID = @ID\r\n        UNION ALL\r\n        SELECT  t.*\r\n        FROM    APZ_OLIN t INNER JOIN\r\n                ret r ON t.FatherLine = r.LineID\r\n)\r\n\r\nSELECT  LineID FROM    ret";
	}

	public string DeleteChildData(int LineID)
	{
		string result = "";
		try
		{
			string query = "apz_MRP_LINES_GetChildID";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@LineID", LineID));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			string text = "";
			if (dataTable != null && dataTable.Rows.Count > 0 && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				text = Function.ToString(dataTable.Rows[0][0]);
				text = ((text[text.Length - 1] == ',') ? text.Remove(text.Length - 1) : text);
			}
			else
			{
				text = LineID.ToString();
			}
			query = "Delete FROM APZ_LIN1 Where \"LineID\" in (" + text + "); \r\n                        Delete FROM APZ_LIN2 Where \"LineID\" in (" + text + ");";
			ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private int AddALine(DataRow r)
	{
		int result = -1;
		try
		{
			int num = Function.ParseInt(r["LineID"]);
			DataTable dataTable = ExistsLine(num);
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			bool flag = false;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				text = "UPDATE APZ_OLIN\r\n                           SET \"Branch\" = @Branch\r\n                           ,\"LineCode\" = @LineCode\r\n                           ,\"LineName\" = @LineName\r\n                           ,\"LineType\" = @LineType\r\n                           ,\"FatherLine\" = @FatherLine\r\n                           ,\"TotalOfMch\" = @TotalOfMch\r\n                           ,\"Perfomance\" = @Perfomance\r\n                    WHERE \"LineID\" = @LineID;";
			}
			else
			{
				flag = true;
				text = "INSERT INTO APZ_OLIN\r\n                           (\"LineID\"\r\n                           ,\"Branch\"\r\n                           ,\"LineCode\"\r\n                           ,\"LineName\"\r\n                           ,\"LineType\"\r\n                           ,\"FatherLine\"\r\n                           ,\"TotalOfMch\"\r\n                           ,\"Perfomance\")\r\n                     VALUES\r\n                           (@LineID\r\n                           ,@Branch\r\n                           ,@LineCode\r\n                           ,@LineName\r\n                           ,@LineType\r\n                           ,@FatherLine\r\n                           ,@TotalOfMch\r\n                           ,@Perfomance);";
			}
			list.Add(new SqlParameter("@LineID", num));
			list.Add(new SqlParameter("@Branch", Function.ParseInt(r["Branch"])));
			list.Add(new SqlParameter("@LineCode", Function.ToString(r["LineCode"])));
			list.Add(new SqlParameter("@LineName", Function.ToString(r["LineName"])));
			list.Add(new SqlParameter("@LineType", Function.ToString(r["LineType"])));
			list.Add(new SqlParameter("@FatherLine", Function.ParseInt(r["FatherLine"])));
			list.Add(new SqlParameter("@TotalOfMch", Function.ParseDouble(r["TotalOfMch"])));
			list.Add(new SqlParameter("@Perfomance", Function.ParseDouble(r["Perfomance"])));
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) == 1)
			{
				if (flag)
				{
					text = "Select max(\"LineID\") From APZ_OLIN;";
					DataTable dataTable2 = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						result = Function.ParseInt(dataTable2.Rows[0][0]);
					}
				}
				else
				{
					result = num;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private string SaveItems(DataTable dt)
	{
		string text = "";
		try
		{
			CreateTempTable(dt, "#APZ_MRP_LINES");
			BulkCopy(dt, "#APZ_MRP_LINES");
			string query = "apz_process_MRP_LINES";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (dataTable.Columns.Contains("ErrorMessage"))
				{
					return Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				}
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "Save Error";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string SaveUsers(DataTable dt)
	{
		string text = "";
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				text = SaveAUser(dt.Rows[i]);
				if (!string.IsNullOrEmpty(text))
				{
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			text = ex.Message;
		}
		return text;
	}

	private string SaveAUser(DataRow r)
	{
		string result = "";
		try
		{
			string query = "INSERT INTO APZ_LIN2\r\n                           (\"LineID\" ,\"User_Code\")\r\n                     VALUES\r\n                           (@LineID ,@User_Code);";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@LineID", Function.ParseInt(r["LineID"])));
			list.Add(new SqlParameter("@User_Code", Function.ToString(r["User_Code"])));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) == 0)
			{
				result = "[SaveAUser] No rows updated";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}
}
