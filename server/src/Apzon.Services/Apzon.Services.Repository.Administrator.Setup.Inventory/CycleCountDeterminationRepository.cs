using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup.Inventory;

namespace Apzon.Services.Repository.Administrator.Setup.Inventory;

public class CycleCountDeterminationRepository : BaseService<DataTable>, ICycleCountDetermination
{
	private const string TempLine = "#templine";

	public CycleCountDeterminationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetList()
	{
		try
		{
			return ExecuteDataTable("APZ_CycleCountDetermination_GetList", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetList  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oCycleCountDetermination, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		Enums.TransactionType transactionType = transtype;
		if ((uint)transactionType <= 1u)
		{
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
	}

	public HttpResult SaveData(DataTable doc, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (doc == null || doc.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable tableStruct = GetTableStruct("OCCS");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			doc = doc.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(doc, "#templine");
			BulkCopy(doc, "#templine");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			if (ExecuteDataTable("APZ_CycleCountDetermination_Save", CommandType.StoredProcedure, sqlParameter) != null)
			{
				return new HttpResult(Enums.MessageCode.Success, "", "Save success!");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
