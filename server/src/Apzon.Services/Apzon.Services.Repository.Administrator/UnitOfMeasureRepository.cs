using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class UnitOfMeasureRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IUnitOfMeasure
{
	public string TempTable = "#OUOM";

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_UoM_GetDoc", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dt)
	{
		return Process(ApzObjectTypes.oUoM, -1, Enums.TransactionType.U, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(document, TempTable);
			BulkCopy(document, TempTable);
			object obj = ExecuteScalar("SP_APZ_OUOM_SAVE", CommandType.StoredProcedure, (SqlParameter[])null);
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

	public DataTable CheckLengthUnit(string unitDisply)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UnitDisply", unitDisply)
			};
			return ExecuteDataTable("APZ_OLGT_GetLineByUnitDisply", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check Length Unit  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckWidthUnit(string unitDisply)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@UnitDisply", unitDisply)
			};
			return ExecuteDataTable("APZ_OWGT_GetLineByUnitDisply", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check Width Unit  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllLengthUnits()
	{
		try
		{
			return ExecuteDataTable("APZ_UoM_GetAllLenUnits", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Doc  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
