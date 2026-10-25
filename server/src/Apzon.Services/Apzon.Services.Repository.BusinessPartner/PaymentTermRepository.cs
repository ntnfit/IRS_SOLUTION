using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class PaymentTermRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IPaymentTerms
{
	private string TempTableName = "#APZ_PAYMENTTERM";

	private string[] _arrStr = new string[39]
	{
		"OCPI;GroupNum;A/P Correction Invoice", "OCPV;GroupNum;A/P Correction Invoice Reversal", "ORPC;GroupNum;A/P Credit Memo", "ODPO;GroupNum;A/P Down Payment", "OPCH;GroupNum;A/P Invoice", "ACRD;GroupNum;Business Partners - History", "OCSI;GroupNum;A/R Correction Invoice", "OCIN;GroupNum;A/R Correction Invoice", "OCSV;GroupNum;A/R Correction Invoice Reversal", "ORIN;GroupNum;A/R Credit Memo",
		"ODPI;GroupNum;A/R Down Payment", "OINV;GroupNum;A/R Invoice", "OOAT;GroupNum;Blanket Agreement", "AOAT;GroupNum;Blanket Agreement", "OCRD;GroupNum;Business Partner", "ODLN;GroupNum;Delivery", "ODRF;GroupNum;Drafts", "OIGE;GroupNum;Goods Issue", "OIGN;GroupNum;Goods Receipt", "OPDN;GroupNum;Goods Receipt PO",
		"ORPD;GroupNum;Goods Return", "OIEI;GroupNum;Incoming Excise Invoice", "CTG1;CTGCode;Installment Layout", "OWTQ;GroupNum;Inventory Transfer Request", "ADOC;GroupNum;Invoice - History", "OSAL;GroupNum;Outgoing", "OOEI;GroupNum;Outgoing Excise Invoice", "OPYM;PaymTerms;Payment Methods for Payment Wizard", "PEX1;DocPrmTerm;Payment Results Table - Rows", "OPOR;GroupNum;Purchase Order",
		"OPRQ;GroupNum;Purchase Request", "ORDN;GroupNum;Returns", "ORDR;GroupNum;Sales Order", "OQUT;GroupNum;Sales Quotation", "OENT;GroupNum;Shipping Types", "AWH1;PmntTerms;Tax Definition", "OTRA;GroupNum;Transition", "WHT1;PmntTerms;Withholding Tax Definition", "OPRQ;GroupNum;Purchase Request"
	};

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("select * from OCTG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataTable dt)
	{
		string text = "";
		try
		{
			return Process(ApzObjectTypes.oPaymentTermsTypes, -1, Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(dt, TempTableName);
			BulkCopy(dt, TempTableName);
			string query = "APZ_PaymentTermSetUp_Save";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][0]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string DeleteDoc(int groupNum)
	{
		string text = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				text = CheckDeleteDoc(groupNum);
				if (string.IsNullOrEmpty(text))
				{
					string query = "DELETE FROM OCTG WHERE \"GroupNum\"=@GroupNum;";
					SqlParameter sqlParameter = new SqlParameter("@GroupNum", groupNum);
					ExecuteNonQuery(query, CommandType.Text, sqlParameter);
					dbTransaction.Commit();
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

	private string CheckDeleteDoc(int groupNum)
	{
		try
		{
			string checkExistString = GetCheckExistString(groupNum);
			if (checkExistString.StartsWith("OK-"))
			{
				checkExistString = checkExistString.Remove(0, 3);
				DataTable dataTable = ExecuteDataTable(checkExistString, CommandType.Text);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
				return "";
			}
			if (checkExistString.StartsWith("ERR-"))
			{
				return checkExistString.Remove(0, 4);
			}
			return checkExistString;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual string GetCheckExistString(int groupNum)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("Declare @GroupNum int = " + groupNum + Environment.NewLine);
			for (int i = 0; i < _arrStr.Length; i++)
			{
				string[] array = _arrStr[i].Split(';');
				if (array != null && array.Length == 3)
				{
					string value = array[0];
					string value2 = array[1];
					string value3 = array[2];
					stringBuilder.Append(Environment.NewLine + "IF EXISTS(SELECT 1 FROM ");
					stringBuilder.Append(value);
					stringBuilder.Append(" t WHERE t.");
					stringBuilder.Append(value2);
					stringBuilder.Append("=@GroupNum)\r\n                            BEGIN\r\n\t                            SELECT 'Cannot remove payment term, linked ");
					stringBuilder.Append(value3);
					stringBuilder.Append(" exist - ");
					stringBuilder.Append(value);
					stringBuilder.Append("' END");
				}
			}
			return stringBuilder.Insert(0, "OK-").ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR-" + ex.Message;
		}
	}

	public DataTable GetPaymentTermInfo(int groupNum)
	{
		try
		{
			string query = "select top 1 a.\"PayDuMonth\", a.\"ExtraMonth\", a.\"ExtraDays\", a.\"BslineDate\", a.\"TolDays\", b.\"InstMonth\", b.\"InstDays\"\r\n                            from OCTG a left join CTG1 b on a.\"GroupNum\" = b.\"CTGCode\"\r\n                            where a.\"GroupNum\" = @groupNum\r\n                            order by b.\"InstMonth\" desc, b.\"InstDays\" desc;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@groupNum", groupNum));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
