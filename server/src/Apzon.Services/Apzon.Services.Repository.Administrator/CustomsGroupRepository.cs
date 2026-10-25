using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class CustomsGroupRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), ICustomsGroup
{
	private string TempTableTab = "#dtOARG";

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OARG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetMaxCstGrpCode()
	{
		try
		{
			return ExecuteDataTable("Select MAX(\"CstGrpCode\") FROM OARG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistCstGrpCode(int cstGrpCode)
	{
		try
		{
			return ExecuteDataTable("select \"ItemCode\" from OITM where \"CstGrpCode\"=@code;", CommandType.Text, new SqlParameter("@code", cstGrpCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveOARG(DataTable dtOARG)
	{
		return Process(ApzObjectTypes.oCustomsGroups, -1, Enums.TransactionType.A, dtOARG);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTableTab);
			BulkCopy(document, TempTableTab);
			object obj = ExecuteScalar("DELETE dbo.OARG\r\n                                     INSERT INTO dbo.OARG\r\n                                     SELECT * FROM #dtOARG \r\n                                     UPDATE ONNM SET AutoKey = (Select MAX(CstGrpCode) FROM OARG) +1 WHERE ObjectCode = '56'", CommandType.Text);
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
