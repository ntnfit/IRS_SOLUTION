using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources.TimeSheeting;

namespace Apzon.Services.Repository.HumanResources.TimeSheeting;

public class AboutRangeTimeRepository : BaseService<DataTable>, IAboutRangeTime
{
	public AboutRangeTimeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OKPG;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		DataTable dataTable = new DataTable();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		{
			if (!dt.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.DatasourceIsMissing);
			}
			int num = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["DocEntry"]));
			for (int num2 = 0; num2 < dt.Rows.Count; num2++)
			{
				if (Function.ParseInt(dt.Rows[num2]["DocEntry"]) == 0)
				{
					dt.Rows[num2]["DocEntry"] = ++num;
				}
			}
			dt.AcceptChanges();
			CreateTempTable(dt, "#APZ_OKPG");
			BulkCopy(dt, "#APZ_OKPG");
			dataTable = ExecuteDataTable("APZ_AboutRangeTime_Save", CommandType.StoredProcedure);
			break;
		}
		case Enums.TransactionType.D:
			dataTable = ExecuteDataTable("APZ_AboutRangeTime_Delete", CommandType.StoredProcedure, new SqlParameter("@docEntry", Function.ParseInt(objId)));
			break;
		default:
			return new HttpResult(Enums.MessageCode.ThisFunctionIsNotSupported, "This function is not supported");
		}
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.None, "Cannot access to database");
		}
		if (Function.ParseInt(dataTable.Rows[0][0]) != 200)
		{
			return new HttpResult(Enums.MessageCode.None, Function.ToString(dataTable.Rows[0][1]));
		}
		return new HttpResult(Enums.MessageCode.Success);
	}
}
