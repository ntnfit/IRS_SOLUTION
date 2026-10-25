using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class PrintLayoutRepository : BaseService<PrintLayout>, IPrintLayout
{
	public PrintLayoutRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("apz_printlayout_get", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public PrintLayout GetDefaultPrintLayout(string userName, string formLayoutCode, string cardCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_printlayout_getdefault", CommandType.StoredProcedure, new SqlParameter("@username", userName), new SqlParameter("@TypeCode", formLayoutCode), new SqlParameter("@CardCode", cardCode ?? ""));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new PrintLayout
				{
					TypeCode = Function.ToString(dataTable.Rows[0]["TypeCode"]),
					TypeName = Function.ToString(dataTable.Rows[0]["TypeName"]),
					DocCode = Function.ToString(dataTable.Rows[0]["DocCode"]),
					DocName = Function.ToString(dataTable.Rows[0]["DocName"]),
					FileName = Function.ToString(dataTable.Rows[0]["FileName"]),
					XtraText = (dataTable.Rows[0].IsExistsColumn("XtraText") ? Function.ToString(dataTable.Rows[0]["XtraText"]) : "")
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new PrintLayout();
	}

	public List<PrintLayout> GetLayouts(string userName, string formLayoutCode, string cardCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_printlayout_getdefault", CommandType.StoredProcedure, new SqlParameter("@username", userName), new SqlParameter("@TypeCode", formLayoutCode), new SqlParameter("@CardCode", cardCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				List<PrintLayout> list = new List<PrintLayout>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					PrintLayout printLayout = new PrintLayout();
					printLayout.TypeCode = Function.ToString(dataTable.Rows[i]["TypeCode"]);
					printLayout.TypeName = Function.ToString(dataTable.Rows[i]["TypeName"]);
					printLayout.DocCode = Function.ToString(dataTable.Rows[i]["DocCode"]);
					printLayout.DocName = Function.ToString(dataTable.Rows[i]["DocName"]);
					printLayout.FileName = Function.ToString(dataTable.Rows[i]["FileName"]);
					printLayout.XtraText = (dataTable.Rows[i].IsExistsColumn("XtraText") ? Function.ToString(dataTable.Rows[i]["XtraText"]) : "");
					list.Add(printLayout);
				}
				return list;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<PrintLayout>();
	}

	public string SaveData(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				string query = "DELETE FROM APZ_PRINTLAYOUT;";
				if (ExecuteNonQuery(query, CommandType.Text) >= 0)
				{
					dt.TableName = "APZ_PRINTLAYOUT";
					if (dt.ExistsColumn("FilePath"))
					{
						dt.Columns.Remove(dt.Columns["FilePath"]);
					}
					string text = InsertDataTableIntoSql(dt);
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
					return string.Empty;
				}
				return "Could not update data Layout";
			}
			return "Datasource is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdatePrinted(string docentry, string table, string type, string userName)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", docentry));
			list.Add(new SqlParameter("@Table", table));
			list.Add(new SqlParameter("@Type", type));
			list.Add(new SqlParameter("@UserName", userName));
			if (ExecuteDataTable("Apz_PrintLayout_UpdatePrinted", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
			{
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Update Printed False!";
	}
}
