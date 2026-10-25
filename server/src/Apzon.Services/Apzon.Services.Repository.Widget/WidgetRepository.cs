using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Widget;

namespace Apzon.Services.Repository.Widget;

public class WidgetRepository : BaseService<DataSet>, IWidget
{
	protected const string TableHeaderName = "APZ_OWID";

	protected const string TableItemName = "APZ_WID1";

	protected const string TablePageName = "APZ_WID2";

	protected const string TempTableHeaderName = "#APZ_OWID";

	protected const string TempTableItemName = "#APZ_WID1";

	protected const string TempTablePageName = "#APZ_WID2";

	public WidgetRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocument(int widgetEntry)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_WID1 WHERE \"AbsEntry\" = @AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", widgetEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetHeader(int widgetEntry)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OWID WHERE \"AbsEntry\" = @AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", widgetEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetItems(int widgetEntry)
	{
		try
		{
			return ExecuteDataTable("apz_widget_chooseitem", CommandType.StoredProcedure, new SqlParameter("@WidgetId", widgetEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult Save(DataSet ds)
	{
		return Process(ApzObjectTypes.oWidget, -1, Enums.TransactionType.U, ds);
	}

	public DataTable GetPages(int widgetEntry)
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_WID2 WHERE \"AbsEntry\" = @AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", widgetEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OWID;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public HttpResult Remove(int absEntry)
	{
		try
		{
			string query = "SELECT 1 FROM APZ_OWID WHERE \"AbsEntry\" = @AbsEntry;";
			if (!ExecuteDataTable(query, CommandType.Text, new SqlParameter("@AbsEntry", absEntry)).IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, Messages.D404);
			}
			return Process(ApzObjectTypes.oWidget, -1, Enums.TransactionType.D, new DataSet(), absEntry.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (Exists(document))
			{
				transtype = Enums.TransactionType.U;
				return UpdateData(document, ref objId, ref objNum, ref transtype);
			}
			transtype = Enums.TransactionType.A;
			return AddNew(document, ref objId, ref objNum, ref transtype);
		case Enums.TransactionType.D:
			if (ExecuteNonQuery("DELETE FROM APZ_OWID WHERE \"AbsEntry\" = @AbsEntry;\r\n                                             DELETE FROM APZ_WID1 WHERE \"AbsEntry\" = @AbsEntry;\r\n                                             DELETE FROM APZ_WID2 WHERE \"AbsEntry\" = @AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", Function.ParseInt(objId))) <= 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "No row affected");
			}
			return new HttpResult();
		default:
			return new HttpResult(Enums.MessageCode.Error, "Unknown error");
		}
	}

	private HttpResult AddNew(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.A;
			int series = -1;
			int cancelEntry = 0;
			InitData(doc, ref objId, ref objNum, ref series, transtype, ref cancelEntry);
			BeforeAction(doc, objId, objNum, transtype, cancelEntry);
			ExecuteNonQuery(parameters: new SqlParameter[1]
			{
				new SqlParameter("@DocEntry", objId)
			}, query: AddNewQuery());
			AfterAction(doc, objId, objNum, transtype, cancelEntry);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	protected virtual string AddNewQuery()
	{
		return string.Format("insert into {0} select * from {1} \r\n                                            update {3} set AbsEntry = @DocEntry\r\n                                            insert into {2} select * from {3}\r\n                                            update {5} set AbsEntry=@DocEntry \r\n                                            insert into {4} select * from {5}", "APZ_OWID", "#APZ_OWID", "APZ_WID1", "#APZ_WID1", "APZ_WID2", "#APZ_WID2");
	}

	private void AfterAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
	}

	private void BeforeAction(DataSet doc, string objId, string objNum, Enums.TransactionType transtype, int cancelEntry)
	{
	}

	private void InitData(DataSet doc, ref string objId, ref string objNum, ref int series, Enums.TransactionType transtype, ref int cancelEntry)
	{
		DataTable dataTable = doc.Tables["APZ_OWID"];
		int num = 0;
		int num2 = Function.ParseInt(dataTable.Rows[0]["AbsEntry"]);
		switch (transtype)
		{
		case Enums.TransactionType.A:
			num = ((num2 < -1) ? num2 : GetNextId());
			dataTable.Rows[0]["AbsEntry"] = num;
			break;
		case Enums.TransactionType.U:
			num = num2;
			break;
		}
		objNum = (objId = num.ToString());
		BulkToTempTable(dataTable, "APZ_OWID", "#APZ_OWID");
		DataTable dataTable2 = doc.Tables["APZ_WID1"];
		if (!dataTable2.IsNotNull())
		{
			throw new Exception("Content is empty");
		}
		BulkToTempTable(dataTable2, "APZ_WID1", "#APZ_WID1");
		DataTable source = ((!doc.Tables.Contains("APZ_WID2")) ? GetPages(0) : doc.Tables["APZ_WID2"]);
		BulkToTempTable(source, "APZ_WID2", "#APZ_WID2");
	}

	private int GetNextId()
	{
		try
		{
			return Function.ParseInt(ExecuteScalar("SELECT Max(\"AbsEntry\") FROM APZ_OWID;", CommandType.Text)) + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	private HttpResult UpdateData(DataSet doc, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.U;
			int series = -1;
			int cancelEntry = 0;
			InitData(doc, ref objId, ref objNum, ref series, transtype, ref cancelEntry);
			BeforeAction(doc, objId, objNum, transtype, cancelEntry);
			ExecuteNonQuery(parameters: new SqlParameter[1]
			{
				new SqlParameter("@DocEntry", objId)
			}, query: UpdateDataQuery());
			AfterAction(doc, objId, objNum, transtype, cancelEntry);
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	protected virtual string UpdateDataQuery()
	{
		return string.Format("delete {0} where AbsEntry=@DocEntry\r\n                                            delete {2} where AbsEntry=@DocEntry  \r\n                                            delete {4} where AbsEntry=@DocEntry  \r\n\r\n                                            insert into {0} select * from {1} \r\n                                            update {3} set AbsEntry=@DocEntry \r\n                                            insert into {2} select * from {3}\r\n                                            update {5} set AbsEntry=@DocEntry\r\n                                            insert into {4} select * from {5}", "APZ_OWID", "#APZ_OWID", "APZ_WID1", "#APZ_WID1", "APZ_WID2", "#APZ_WID2");
	}

	private bool Exists(DataSet doc)
	{
		try
		{
			if (!doc.Tables.Contains("APZ_OWID"))
			{
				throw new Exception("[Check exists] - Could not get data - header name");
			}
			if (!doc.Tables["APZ_OWID"].IsNotNull())
			{
				throw new Exception("[Check exists] - Could not get data - header");
			}
			string query = "SELECT 1 FROM APZ_OWID WHERE \"AbsEntry\" =" + Function.ParseInt(doc.Tables["APZ_OWID"].Rows[0]["AbsEntry"]) + ";";
			return ExecuteDataTable(query, CommandType.Text).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}
}
