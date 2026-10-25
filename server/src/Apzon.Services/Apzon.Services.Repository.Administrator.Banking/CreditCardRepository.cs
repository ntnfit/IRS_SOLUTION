using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Banking;

namespace Apzon.Services.Repository.Administrator.Banking;

public class CreditCardRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ICreditCard
{
	public string TempTable = "#OCRC";

	private readonly string connection;

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_CreditCard_GetDocs", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataTable dt)
	{
		return Process(ApzObjectTypes.oCreditCards, -1, Enums.TransactionType.U, dt).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			object obj = ExecuteScalar("SP_APZ_OCRC_SAVE", CommandType.Text);
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
}
