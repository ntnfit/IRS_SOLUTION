using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class FormWizardRepository : BaseService<DataSet>, IFormWizard
{
	private const string TempTableDoc = "#APZ_OFWZ";

	private const string TempTableContainer = "#APZ_FWZ1";

	private const string TempTableFields = "#APZ_FWZ2";

	private const string TempTableValidValues = "#APZ_FWZ3";

	private const string TableDoc = "APZ_OFWZ";

	private const string TableContainer = "APZ_FWZ1";

	private const string TableFields = "APZ_FWZ2";

	private const string TableValidValues = "APZ_FWZ3";

	public FormWizardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetValid()
	{
		try
		{
			return ExecuteDataTable(GetValidQuery(), CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetValidQuery()
	{
		return "SELECT * FROM APZ_OFWZ WHERE \"Valid\"='Y' ORDER BY \"Caption\"";
	}

	public DataSet GetById(int templateId)
	{
		DataSet dataSet = new DataSet();
		string query = "SELECT * FROM APZ_OFWZ WHERE \"AbsEntry\"=@Id;";
		DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Id", templateId));
		if (dataTable != null)
		{
			dataTable.TableName = "OFWZ";
			dataSet.Tables.Add(dataTable.Copy());
		}
		query = "SELECT * FROM APZ_FWZ1 WHERE \"AbsEntry\"=@Id;";
		DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Id", templateId));
		if (dataTable2 != null)
		{
			dataTable2.TableName = "FWZ1";
			dataSet.Tables.Add(dataTable2.Copy());
		}
		query = "SELECT * FROM APZ_FWZ2 WHERE \"AbsEntry\"=@Id ORDER BY \"VisIndex\";";
		DataTable dataTable3 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Id", templateId));
		if (dataTable3 != null)
		{
			dataTable3.TableName = "FWZ2";
			dataSet.Tables.Add(dataTable3.Copy());
		}
		query = "SELECT * FROM APZ_FWZ3 WHERE \"AbsEntry\"=@Id;";
		DataTable dataTable4 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Id", templateId));
		if (dataTable4 != null)
		{
			dataTable4.TableName = "FWZ3";
			dataSet.Tables.Add(dataTable4.Copy());
		}
		return dataSet;
	}

	public DataTable GetAll()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM APZ_OFWZ ORDER BY \"Caption\"", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public HttpResult SaveData(DataSet ds)
	{
		try
		{
			return Process(ApzObjectTypes.oFormWizard, -1, Enums.TransactionType.U, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(int absEntry)
	{
		try
		{
			return Process(ApzObjectTypes.oFormWizard, -1, Enums.TransactionType.D, new DataSet(), absEntry.ToString());
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
			return SaveData(document, ref transtype);
		case Enums.TransactionType.D:
			return RemoveData(objId);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Unknown error");
		}
	}

	private HttpResult RemoveData(string objId)
	{
		try
		{
			return (ExecuteNonQuery("DELETE FROM APZ_OFWZ WHERE \"AbsEntry\"=@AbsEntry; \r\n                                     DELETE FROM APZ_FWZ1 WHERE \"AbsEntry\"=@AbsEntry;\r\n                                     DELETE FROM APZ_FWZ2 WHERE \"AbsEntry\"=@AbsEntry;\r\n                                     DELETE FROM APZ_FWZ3 WHERE \"AbsEntry\"=@AbsEntry;", CommandType.Text, new SqlParameter("@AbsEntry", Function.ParseInt(objId))) != 0) ? new HttpResult(Enums.MessageCode.Success) : new HttpResult(Enums.MessageCode.Error, "No row affected");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private HttpResult SaveData(DataSet doc, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.U;
			BulkToTempTable(doc.Tables["OFWZ"], "APZ_OFWZ", "#APZ_OFWZ");
			BulkToTempTable(doc.Tables["FWZ1"], "APZ_FWZ1", "#APZ_FWZ1");
			BulkToTempTable(doc.Tables["FWZ2"], "APZ_FWZ2", "#APZ_FWZ2");
			BulkToTempTable(doc.Tables["FWZ3"], "APZ_FWZ3", "#APZ_FWZ3");
			string query = "apz_form_wizard_process";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@transtype", transtype.ToString()));
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(dataTable.Rows[0][0]);
				if (num > 0)
				{
					string text = Function.ToString(dataTable.Rows[0][1]);
					throw new Exception(num + "-" + text);
				}
			}
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}
}
