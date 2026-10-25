using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.RoutingAndResource;

namespace Apzon.Services.Repository.RoutingAndResource;

public class CapacityRepository : BaseService<DataTable>, ICapacity
{
	public CapacityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetResources()
	{
		try
		{
			return ExecuteDataTable("APZ_Capacity_GetResource", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetDataAppointments(int scheduleId, Enums.TransactionType transactionType)
	{
		try
		{
			return ExecuteDataTable("APZ_Capacity_Run", CommandType.StoredProcedure, new SqlParameter("@scheduleId", scheduleId), new SqlParameter("@tranType", Function.ToString(transactionType)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetColorSetting()
	{
		try
		{
			return ExecuteDataTable("APZ_Capacity_GetColorSetting", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public string Save(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oCapacity, -1, Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		if (transtype == Enums.TransactionType.D)
		{
			return new HttpResult();
		}
		return SaveData(document);
	}

	public HttpResult SaveData(DataTable dt)
	{
		if (!dt.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not null");
		}
		DataTable tableStruct = GetTableStruct("APZ_CGRC");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dt, "#tempDataCp");
		BulkCopy(dt, "#tempDataCp");
		DataTable dataTable = ExecuteDataTable("APZ_Capacity_Save", CommandType.StoredProcedure);
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
			{
				return new HttpResult(Enums.MessageCode.Success);
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult(Enums.MessageCode.Error, "Save error");
	}

	public DataTable GetDataResourceEffic(string resCode, string routCode, int routLine)
	{
		try
		{
			return ExecuteDataTable("APZ_Capacity_GetDataResourceEffic", CommandType.StoredProcedure, new SqlParameter("@resCode", resCode), new SqlParameter("@routCode", routCode), new SqlParameter("@routLine", routLine));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}
}
