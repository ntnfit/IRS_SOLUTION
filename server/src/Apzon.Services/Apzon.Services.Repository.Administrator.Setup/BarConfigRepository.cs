using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class BarConfigRepository : BaseService<DataTable>, IBarConfig
{
	private const string Table = "#temp";

	public BarConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OCPB;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetColumns()
	{
		try
		{
			return ExecuteDataTable(GetColumnsQuery(), CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetColumnsQuery()
	{
		return "select top 0 * into #temp from APZ_OCPB insert into #temp (ObjType) values (202) insert into #temp (ObjType) values (22) select * from #temp drop table #temp";
	}

	public string Save(DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return "Data could null!";
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, "#temp");
				BulkCopy(dt, "#temp");
				if (ExecuteNonQuery(GetSaveQuery(), CommandType.Text) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "Save error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetSaveQuery()
	{
		return "delete from APZ_OCPB a where exists(select 1 from #temp \"ObjType\" = a.\"ObjType\");\r\n                                            insert into APZ_OCPB select * from #temp;";
	}
}
