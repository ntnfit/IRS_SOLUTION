using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.RoutingAndResource;

namespace Apzon.Services.Repository.RoutingAndResource;

public class CapacitySettingRepository : BaseService<DataSet>, ICapacitySetting
{
	public CapacitySettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetColumnSetting()
	{
		try
		{
			return ExecuteDataTable("SELECT COLUMN_NAME \"Code\", COLUMN_NAME \"Name\" FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='APZ_CGRC'", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataSet GetDataSetting()
	{
		try
		{
			return ExecuteData("SELECT * from APZ_CPCS\r\n                      SELECT * FROM APZ_CPCS1", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataSet();
		}
	}

	public HttpResult SaveData(DataSet dsSave)
	{
		try
		{
			return Process(ApzObjectTypes.oCapacitySetting, -1, Enums.TransactionType.A, dsSave);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		if (transtype == Enums.TransactionType.A)
		{
			if (dsSave == null || dsSave.Tables.Count != 2 || !dsSave.Tables[0].IsNotNull() || !dsSave.Tables[1].IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Data save can not null");
			}
			CreateTempTable(dsSave.Tables[0], "#tempGeneral");
			BulkCopy(dsSave.Tables[0], "#tempGeneral");
			CreateTempTable(dsSave.Tables[1], "#tempToolTip");
			BulkCopy(dsSave.Tables[1], "#tempToolTip");
			ExecuteNonQuery("APZ_CapacitySetting_Save", CommandType.StoredProcedure);
			return new HttpResult();
		}
		throw new Exception("Function not support");
	}
}
