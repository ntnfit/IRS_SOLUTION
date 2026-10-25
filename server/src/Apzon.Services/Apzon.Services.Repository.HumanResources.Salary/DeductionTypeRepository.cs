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
using Apzon.Services.Interfaces.HumanResources.Salary;

namespace Apzon.Services.Repository.HumanResources.Salary;

public class DeductionTypeRepository : BaseService<DataSet>, IDeductionType
{
	private const string Doctemp = "#APZ_OHDT";

	private const string Linetemp = "#APZ_HDT1";

	public DeductionTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetList()
	{
		try
		{
			string query = "Select * from APZ_OHDT Order by \"Name\";";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable FindDataNextLast(int id, int type)
	{
		try
		{
			string query = "APZ_DeductionType_FindNextLastFrist";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@type", type));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "FindDataNextLast  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDetail(int code)
	{
		try
		{
			string query = "select * from APZ_HDT1 where \"Code\" = @code Order by \"ValidFrom\" ASC;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT MAX(\"Code\") FROM APZ_OHDT;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxCode FAILED ", ex.Message);
			return 0;
		}
	}

	public DataTable GetByDate(DateTime fromDate, DateTime toDate)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@fromDate", fromDate));
			list.Add(new SqlParameter("@toDate", toDate));
			return ExecuteDataTable("APZ_DeductionType_GetByDate", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetByDate  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public HttpResult Save(DataSet ds, string action)
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
			return Process(ApzObjectTypes.oDeductionType, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(ref ret, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable dataTable = ds.Tables[0];
			DataTable tableStruct = GetTableStruct("APZ_OHDT");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_OHDT");
			BulkCopy(dataTable, "#APZ_OHDT");
			DataTable dataTable2 = ds.Tables[1];
			DataTable tableStruct2 = GetTableStruct("APZ_HDT1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#APZ_HDT1");
			BulkCopy(dataTable2, "#APZ_HDT1");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable3 = ExecuteDataTable("APZ_DeductionType_Saves", CommandType.StoredProcedure, sqlParameter);
			if (dataTable3 != null && dataTable3.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable3.Rows[0][1])))
				{
					objId = Function.ToString(dataTable3.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable3.Rows[0][0])}-{Function.ToString(dataTable3.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string DeleteDocs(string ids)
	{
		try
		{
			return Process(ApzObjectTypes.oDeductionType, -1, Enums.TransactionType.D, new DataSet(), ids).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ids", objId);
			if (ExecuteNonQuery("APZ_DeductionType_DeleteDocs", CommandType.StoredProcedure, sqlParameter) > 0)
			{
				return new HttpResult(Enums.MessageCode.Success, "", "");
			}
			return new HttpResult(Enums.MessageCode.Error, "", "Delete Error!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
