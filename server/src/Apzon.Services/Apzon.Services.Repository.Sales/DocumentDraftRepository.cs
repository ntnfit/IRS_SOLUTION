using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.Sales;

namespace Apzon.Services.Repository.Sales;

public class DocumentDraftRepository(IDatabaseService databaseService) : BaseService<DocumentsImpl>(databaseService), IDocumentDraft
{
	private string temptb = "#TEMPTB";

	public DataTable GetTableDefault()
	{
		try
		{
			string query = "APZ_DocumentDraft_GetDefaultTable";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDefaultTable  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public ResponseResult DMSSearchDoc(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			string query = "APZ_D_GetDocDraft";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", Function.ToString(dataRow["UserLogin"])));
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["ObjType"])))
			{
				list.Add(new SqlParameter("@ObjType", Function.ToString(dataRow["ObjType"])));
			}
			else
			{
				list.Add(new SqlParameter("@ObjType", DBNull.Value));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["UserSign"])))
			{
				list.Add(new SqlParameter("@UserID", Function.ToString(dataRow["UserSign"])));
			}
			else
			{
				list.Add(new SqlParameter("@UserID", DBNull.Value));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["DocStatus"])))
			{
				list.Add(new SqlParameter("@OpenOnly", Function.ToString(dataRow["DocStatus"])));
			}
			else
			{
				list.Add(new SqlParameter("@OpenOnly", DBNull.Value));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Date"])) && Function.ToString(dataRow["Date"]) != "0")
			{
				list.Add(new SqlParameter("@Date", Function.ToString(dataRow["Date"])));
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["TypeDate"])))
				{
					list.Add(new SqlParameter("@TypeDate", Function.ToString(dataRow["TypeDate"])));
				}
				else
				{
					list.Add(new SqlParameter("@TypeDate", DBNull.Value));
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["From"])))
				{
					list.Add(new SqlParameter("@FromValue", Function.ToString(dataRow["From"])));
				}
				else
				{
					list.Add(new SqlParameter("@FromValue", DBNull.Value));
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["To"])))
				{
					list.Add(new SqlParameter("@ToValue", Function.ToString(dataRow["To"])));
				}
				else
				{
					list.Add(new SqlParameter("@ToValue", DBNull.Value));
				}
			}
			else
			{
				list.Add(new SqlParameter("@Date", DBNull.Value));
				list.Add(new SqlParameter("@TypeDate", DBNull.Value));
				list.Add(new SqlParameter("@FromValue", DBNull.Value));
				list.Add(new SqlParameter("@ToValue", DBNull.Value));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["PageIndex"])))
			{
				list.Add(new SqlParameter("@pageIndex", Function.ToString(dataRow["PageIndex"])));
			}
			else
			{
				list.Add(new SqlParameter("@pageIndex", DBNull.Value));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["SearchText"])))
			{
				list.Add(new SqlParameter("@searchText", Function.ToString(dataRow["SearchText"])));
			}
			else
			{
				list.Add(new SqlParameter("@searchText", DBNull.Value));
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			string filter = string.Empty;
			if (dt.ExistsColumn("FilterString"))
			{
				filter = Function.ToString(dt.Rows[0]["FilterString"]);
			}
			string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (!string.IsNullOrEmpty(filterText))
				{
					DataRow[] array = dataTable.Select(filterText);
					if (array.Length != 0)
					{
						return new ResponseResult
						{
							MessageCode = "200",
							Message = "",
							Content = array.CopyToDataTable()
						};
					}
					return new ResponseResult
					{
						MessageCode = "200",
						Message = "",
						Content = new DataTable()
					};
				}
				return new ResponseResult
				{
					MessageCode = "200",
					Message = "",
					Content = dataTable
				};
			}
			return new ResponseResult
			{
				MessageCode = "200",
				Message = "",
				Content = new DataTable()
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDoc  FAILED ", ex.Message);
			return new ResponseResult
			{
				MessageCode = "402",
				Message = ex.Message
			};
		}
	}

	public DataTable SearchDoc(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			string query = "SP_APZ_GetDocDraft";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", Function.ToString(dataRow["UserLogin"])));
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["ObjType"])))
			{
				list.Add(new SqlParameter("@ObjType", Function.ToString(dataRow["ObjType"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["UserSign"])))
			{
				list.Add(new SqlParameter("@UserID", Function.ToString(dataRow["UserSign"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["DocStatus"])))
			{
				list.Add(new SqlParameter("@OpenOnly", Function.ToString(dataRow["DocStatus"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Date"])) && Function.ToString(dataRow["Date"]) != "0")
			{
				list.Add(new SqlParameter("@Date", Function.ToString(dataRow["Date"])));
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["TypeDate"])))
				{
					list.Add(new SqlParameter("@TypeDate", Function.ToString(dataRow["TypeDate"])));
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["From"])))
				{
					list.Add(new SqlParameter("@FromValue", Function.ToString(dataRow["From"])));
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["To"])))
				{
					list.Add(new SqlParameter("@ToValue", Function.ToString(dataRow["To"])));
				}
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			string filter = string.Empty;
			if (dt.ExistsColumn("FilterString"))
			{
				filter = Function.ToString(dt.Rows[0]["FilterString"]);
			}
			string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (!string.IsNullOrEmpty(filterText))
				{
					DataRow[] array = dataTable.Select(filterText);
					if (array.Length != 0)
					{
						return array.CopyToDataTable();
					}
					return new DataTable();
				}
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchListDocEntry(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["ObjType"])))
			{
				list.Add(new SqlParameter("@ObjType", Function.ToString(dataRow["ObjType"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["UserSign"])))
			{
				list.Add(new SqlParameter("@UserID", Function.ToString(dataRow["UserSign"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["DocStatus"])))
			{
				list.Add(new SqlParameter("@OpenOnly", Function.ToString(dataRow["DocStatus"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["Date"])) && Function.ToString(dataRow["Date"]) != "0")
			{
				list.Add(new SqlParameter("@Date", Function.ToString(dataRow["Date"])));
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["TypeDate"])))
				{
					list.Add(new SqlParameter("@TypeDate", Function.ToString(dataRow["TypeDate"])));
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["From"])))
				{
					list.Add(new SqlParameter("@FromValue", Function.ToString(dataRow["From"])));
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataRow["To"])))
				{
					list.Add(new SqlParameter("@ToValue", Function.ToString(dataRow["To"])));
				}
			}
			DataTable dataTable = ExecuteDataTable("SP_APZ_SearchListDocEntry", CommandType.StoredProcedure, list.ToArray());
			string filter = string.Empty;
			if (dt.ExistsColumn("FilterString"))
			{
				filter = Function.ToString(dt.Rows[0]["FilterString"]);
			}
			string filterText = GetUnitOfWork().FilterSetting.GetFilterText(filter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (!string.IsNullOrEmpty(filterText))
				{
					DataRow[] array = dataTable.Select(filterText);
					if (array.Length != 0)
					{
						return array.CopyToDataTable();
					}
					return new DataTable();
				}
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchLine(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			if (Function.ToString(dataRow["DocEntry"]) != "0")
			{
				list.Add(new SqlParameter("@docEntry", Function.ToString(dataRow["DocEntry"])));
			}
			DataTable dataTable = ExecuteDataTable("SP_APZ_GetLineDraft", CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchLine  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string RemoveAndClose(DataTable dt)
	{
		return "";
	}

	public DataTable GetDraftByEntry(int docEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@docEntry", docEntry)
			};
			return ExecuteDataTable("Select * from ODRF where \"DocEntry\" = @docEntry;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string CheckIsIns(int docEntry)
	{
		try
		{
			return Function.ToString(ExecuteScalar("Select \"isIns\" from ODRF where \"DocEntry\" = @docEntry;", CommandType.Text, new SqlParameter("@docEntry", docEntry)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public string SaveLogForm(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, temptb);
				BulkCopy(dt, temptb);
				string result = Function.ToString(ExecuteScalar("APZ_SaveLogForm", CommandType.StoredProcedure));
				dbTransaction.Commit();
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public DataTable GetLogForm(string userName)
	{
		try
		{
			string query = "select * from APZ_LOGFORM where userName= @userName;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@userName", userName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetApprovalInfo(int docEntry)
	{
		try
		{
			return ExecuteDataTable("APZ_Draft_GetApprovalInfo", CommandType.StoredProcedure, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
			return new DataTable();
		}
	}
}
