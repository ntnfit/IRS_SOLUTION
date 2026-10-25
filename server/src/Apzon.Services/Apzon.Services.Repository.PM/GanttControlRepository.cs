using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PM;

namespace Apzon.Services.Repository.PM;

internal class GanttControlRepository : BaseService<DataTable>, IGanttControl
{
	public GanttControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadByProject(int projectAbsEntry)
	{
		try
		{
			return ExecuteDataTable("apz_GanttControl_LoadByProject", CommandType.StoredProcedure, new SqlParameter("@projectAbsEntry", projectAbsEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public HttpResult UpdateTask(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oGanttControl, -1, Enums.TransactionType.U, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult
			{
				Message = ex.Message,
				MessageCode = Enums.MessageCode.Error
			};
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(document, "#GanttTask");
			BulkCopy(document, "#GanttTask");
			DataTable dataTable = ExecuteDataTable("apz_GanttControl_UpdateTask", CommandType.StoredProcedure);
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(dataTable.Rows[0][0]);
				string message = Function.ToString(dataTable.Rows[0][1]);
				if (num != 0)
				{
					return new HttpResult
					{
						Message = message,
						MessageCode = Enums.MessageCode.Error
					};
				}
			}
			objId = (objNum = string.Join(",", (from t in document.AsEnumerable()
				select Function.ParseInt(t["ClgCode"])).ToArray()));
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult
			{
				Message = ex.Message,
				MessageCode = Enums.MessageCode.Error
			};
		}
	}
}
