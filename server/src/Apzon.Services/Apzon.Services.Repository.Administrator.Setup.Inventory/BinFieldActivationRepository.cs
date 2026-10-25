using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class BinFieldActivationRepository : BaseService<IBinFieldActivation>, IBinFieldActivation
{
	private const string TempTable = "#dtOBFC";

	public BinFieldActivationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetObfcData()
	{
		try
		{
			return ExecuteDataTable("select \"AbsEntry\", \"FldType\", \"FldNum\", \"DispName\", \"Activated\", \"DftName\" from OBFC order by \"AbsEntry\" asc;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string IsFieldExist(int absEntry, string fldType)
	{
		string result = "";
		try
		{
			SqlParameter[] parameters = new SqlParameter[2]
			{
				new SqlParameter("@absEntry", absEntry),
				new SqlParameter("@fldType", fldType)
			};
			DataTable dataTable = ExecuteDataTable(GetQueryFieldExist(), CommandType.Text, parameters);
			if (dataTable.IsNotNull())
			{
				result = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	protected virtual string GetQueryFieldExist()
	{
		return "if (@fldType='S' and exists (select top 1 1 from OBSL where FldAbs = @absEntry)) or \r\n                    (@fldType='A' and exists(select top 1 1 from OBAT where FldAbs=@absEntry))\r\n\t                begin\r\n\t\t                select 'Exist'\r\n\t\t                return\r\n\t                end\r\n\r\n\t                select ''";
	}

	public string Save(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, "#dtOBFC");
				BulkCopy(dt, "#dtOBFC");
				ExecuteNonQuery(GetQuerySave(), CommandType.Text);
				dbTransaction.Commit();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetQuerySave()
	{
		return "update a set DispName=f.DispName, Activated=f.Activated, UserSign=ISNULL(a.UserSign,f.UserSign), UserSign2=f.UserSign, UpdateDate=GETDATE() \r\n\t                from OBFC a inner join #dtOBFC f on a.AbsEntry=f.AbsEntry";
	}
}
