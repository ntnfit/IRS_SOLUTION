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
using Apzon.Services.Interfaces.FacebookCRM;

namespace Apzon.Services.Repository.FacebookCRM;

public class FacebookCRMRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IFacebookCRM
{
	private string processType = "";

	private string TempTableTab = "#dtAPZ_OCRM";

	public DataTable LoadMessages(string pageId)
	{
		try
		{
			string query = "select * from APZ_OCRM WHERE PageId=@PageId";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@PageId", pageId));
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

	public DataTable LoadMessagesForCustomer(string pageId, string customerId)
	{
		try
		{
			string query = "select * from APZ_OCRM WHERE PageId=@PageId AND CustomerId=@CustomerId";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@PageId", pageId));
			list.Add(new SqlParameter("@CustomerId", customerId));
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

	public DataTable LoadCustomers(string pageId)
	{
		try
		{
			string query = "APZ_FacebookCRM_LoadCustomers";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@PageId", pageId));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
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

	public DataTable LoadPageInfo(string pageId)
	{
		try
		{
			string query = "select * from APZ_CRMP WHERE PageId=@PageId";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@PageId", pageId));
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

	public DataTable LoadPageInfoFromUser(string userId)
	{
		try
		{
			string query = "select * from APZ_CRMP WHERE UserID=@UserID";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserID", userId));
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

	public DataTable LoadOrderAndItemCode()
	{
		try
		{
			string query = "select DocEntry,ItemCode from RDR1";
			List<SqlParameter> list = new List<SqlParameter>();
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

	public int GetDocNumByDocEntry(int docEntry)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select DocNum from ORDR where DocEntry = @docEntry", CommandType.Text, new SqlParameter("@docEntry", docEntry));
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return 0;
			}
			return Function.ParseInt(dataTable.Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public string SaveUser(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction(IsolationLevel.RepeatableRead);
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserID", Function.ToString(dt.Rows[0]["UserID"])));
				list.Add(new SqlParameter("@UserName", Function.ToString(dt.Rows[0]["UserName"])));
				list.Add(new SqlParameter("@AccessToken", Function.ToString(dt.Rows[0]["AccessToken"])));
				list.Add(new SqlParameter("@LastLog", Function.ParseDateTimes(dt.Rows[0]["LastLog"])));
				if (ExecuteNonQuery("APZ_FacebookCRM_SaveUser", CommandType.StoredProcedure, list.ToArray()) <= 0)
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

	public string ChangePageStatus(string pageId, DateTime lastLog)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction(IsolationLevel.RepeatableRead);
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@PageId", pageId));
				list.Add(new SqlParameter("@LastLog", lastLog));
				if (ExecuteNonQuery("UPDATE APZ_CRMP SET Actived='Y',LastLog=@LastLog WHERE PageId=@PageId", CommandType.Text, list.ToArray()) <= 0)
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

	public string SavePageList(DataTable dt)
	{
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		processType = "SavePageList";
		return Process(ApzObjectTypes.oFacebookCRM, -1, Enums.TransactionType.A, dataSet).Message;
	}

	public string SaveCustomers(DataSet dt)
	{
		processType = "SaveCustomers";
		return Process(ApzObjectTypes.oFacebookCRM, -1, Enums.TransactionType.A, dt).Message;
	}

	public string ActiveMessages(DataTable dt)
	{
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		processType = "ActiveMessages";
		return Process(ApzObjectTypes.oFacebookCRM, -1, Enums.TransactionType.A, dataSet).Message;
	}

	public string SaveMessages(DataTable dt)
	{
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		processType = "SaveMessages";
		return Process(ApzObjectTypes.oFacebookCRM, -1, Enums.TransactionType.A, dataSet).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			if (document.Tables.Count > 0)
			{
				switch (processType)
				{
				case "SaveMessages":
				{
					CreateTempTable(document.Tables[0], TempTableTab);
					BulkCopy(document.Tables[0], TempTableTab);
					text = "APZ_FacebookCRM_SaveMessages";
					object obj4 = ExecuteScalar(text, CommandType.StoredProcedure);
					objId = Function.ToString(obj4);
					objNum = objId;
					break;
				}
				case "ActiveMessages":
				{
					CreateTempTable(document.Tables[0], TempTableTab);
					BulkCopy(document.Tables[0], TempTableTab);
					text = "APZ_FacebookCRM_ActiveMessages";
					object obj3 = ExecuteScalar(text, CommandType.StoredProcedure);
					objId = Function.ToString(obj3);
					objNum = objId;
					break;
				}
				case "SaveCustomers":
				{
					if (document.Tables.Contains("Customers") && document.Tables["Customers"].IsNotNull())
					{
						string tblName2 = "#dtAPZ_CRMC";
						CreateTempTable(document.Tables["Customers"], tblName2);
						BulkCopy(document.Tables["Customers"], tblName2);
					}
					if (document.Tables.Contains("Messages") && document.Tables["Messages"].IsNotNull())
					{
						string tblName3 = "#dtAPZ_OCRM";
						CreateTempTable(document.Tables["Messages"], tblName3);
						BulkCopy(document.Tables["Messages"], tblName3);
					}
					text = "APZ_FacebookCRM_SaveCustomers";
					object obj2 = ExecuteScalar(text, CommandType.StoredProcedure);
					objId = Function.ToString(obj2);
					objNum = objId;
					break;
				}
				case "SavePageList":
				{
					string tblName = "#dtAPZ_CRMP";
					CreateTempTable(document.Tables[0], tblName);
					BulkCopy(document.Tables[0], tblName);
					text = "APZ_FacebookCRM_SavePageList";
					object obj = ExecuteScalar(text, CommandType.StoredProcedure);
					objId = Function.ToString(obj);
					objNum = objId;
					break;
				}
				}
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
