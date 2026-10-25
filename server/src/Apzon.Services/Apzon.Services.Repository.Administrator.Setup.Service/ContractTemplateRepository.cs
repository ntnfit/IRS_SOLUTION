using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup.Service;

namespace Apzon.Services.Repository.Administrator.Setup.Service;

public class ContractTemplateRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IContractTemplate
{
	private string _dtTmpName = "#APZ_ContractTPL";

	private string _dtATCTmpName = "#APZ_ContractATC";

	public DataTable CheckDoc(string tmpltName)
	{
		try
		{
			return ExecuteDataTable("Select 1 from OCTT where tmpltName=@tmpltName", CommandType.Text, new SqlParameter("@tmpltName", tmpltName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetData(string tmpltName)
	{
		try
		{
			return ExecuteDataTable("Select * from OCTT where tmpltName=@tmpltName", CommandType.Text, new SqlParameter("@tmpltName", tmpltName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRecord(string position, string tmpltName)
	{
		try
		{
			return ExecuteDataTable("apz_ContractTemplate_GetRecord", CommandType.StoredProcedure, new SqlParameter("@position", position), new SqlParameter("@tmpltName", tmpltName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataSet ds)
	{
		try
		{
			if (ds == null || ds.Tables.Count != 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return "No row to save";
			}
			string text = Function.ToString(ds.Tables[0].Rows[0]["SaveAction"]);
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (text)
			{
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			default:
				return "Invalid save action '" + text + "' must be: 'A'-Add; 'U'-Update; 'D'-Delete;";
			case "A":
				break;
			}
			return Process(ApzObjectTypes.oContractTemplates, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter[] array = new SqlParameter[2];
			string value = Function.ToString(ds.Tables[0].Rows[0]["OldName"]);
			array[0] = new SqlParameter("@OldCode", value);
			string value2 = "";
			switch (transtype)
			{
			case Enums.TransactionType.A:
				value2 = "A";
				break;
			case Enums.TransactionType.U:
				value2 = "U";
				break;
			case Enums.TransactionType.D:
				value2 = "D";
				break;
			}
			array[1] = new SqlParameter("@TransType", value2);
			ds.Tables[0].Columns.Remove("OldName");
			ds.Tables[0].Columns.Remove("SaveAction");
			ds.Tables[0].AcceptChanges();
			CreateTempTable(ds.Tables[0], _dtTmpName);
			BulkCopy(ds.Tables[0], _dtTmpName);
			CreateTempTable(ds.Tables[1], _dtATCTmpName);
			BulkCopy(ds.Tables[1], _dtATCTmpName);
			DataTable dataTable = ExecuteDataTable("apz_ContractTemplate_Process", CommandType.StoredProcedure, array);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][0]));
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet Search(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			return ExecuteData("apz_ContractTemplate_Search", CommandType.StoredProcedure, new SqlParameter("@TmpltName", Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["TmpltName"]))), new SqlParameter("@CntrctType", Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["CntrctType"]))), new SqlParameter("@ResponseV", Function.ParseInt(dataRow["ResponseV"])), new SqlParameter("@ResponseU", Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["ResponseU"]))), new SqlParameter("@Descriptio", Function.ToString(dataRow["Descriptio"])), new SqlParameter("@Deleted", Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["Deleted"]))), new SqlParameter("@Renewal", Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["Renewal"]))), new SqlParameter("@Duration", Function.ParseInt(dataRow["Duration"])), new SqlParameter("@RemindVal", Function.ParseInt(dataRow["RemindVal"])), new SqlParameter("@RemindUnit", Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["RemindUnit"]))), new SqlParameter("@Remark", Function.ReplaceSQLInjectCharInCode(Function.ToString(dataRow["Remark"]))));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}
}
