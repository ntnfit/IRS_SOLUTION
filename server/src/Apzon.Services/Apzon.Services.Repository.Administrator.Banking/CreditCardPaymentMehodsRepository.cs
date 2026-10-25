using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Banking;

namespace Apzon.Services.Repository.Administrator.Banking;

public class CreditCardPaymentMehodsRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ICreditCardPaymentMethods
{
	public string TempTable = "#OCRP";

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_CreditPaymentMethods_GetDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataTable dtDoc)
	{
		return Process(ApzObjectTypes.oCreditPaymentMethods, -1, Enums.TransactionType.U, dtDoc).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			object obj = ExecuteScalar("SP_APZ_OCRP_SAVE", CommandType.Text);
			objId = Function.ToString(obj);
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetCreditCards()
	{
		try
		{
			return ExecuteDataTable("select \"CreditCard\" \"Code\", \"CardName\" \"Name\" from OCRC;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetPaymentCodes()
	{
		try
		{
			return ExecuteDataTable("select \"Code\", \"Name\" from OCDT;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string DeleteLine(int crTypeCode)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter sqlParameter = new SqlParameter("@crTypeCode", crTypeCode);
				if (ExecuteNonQuery("APZ_Service_CreditPaymentMethods_DeleteLine", CommandType.StoredProcedure, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Delete Error!";
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
