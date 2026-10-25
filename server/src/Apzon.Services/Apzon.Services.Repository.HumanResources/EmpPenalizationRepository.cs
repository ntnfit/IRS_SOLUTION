using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class EmpPenalizationRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IEmpPenalization
{
	private string Temp_Penalization = "#APZ_HPT1";

	private string Temp_Doc = "#APZ_HPT2";

	public DataTable GetPeriodRecords()
	{
		try
		{
			return ExecuteDataTable("APZ_Penalization_GetDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetPeriodRecords  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetData(int docNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocNo", Function.ParseInt(docNum)));
			return ExecuteDataTable("APZ_Penalization_GetLine", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadEmpPenalization  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oPenalization, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Remove(int docNo)
	{
		try
		{
			return Process(ApzObjectTypes.oPenalization, -1, Enums.TransactionType.D, new DataSet(), Function.ToString(docNo)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDelete(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet dtS, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!dtS.IsNotNull())
			{
				ret.Message = "No data to save";
				return;
			}
			_ = transtype;
			DataTable dataTable = dtS.Tables["dtLine"];
			DataTable tableStruct = GetTableStruct("APZ_HPT1");
			List<int> list = new List<int>();
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				DataColumn dataColumn = dataTable.Columns[i];
				if (!tableStruct.Columns.Contains(dataColumn.ColumnName))
				{
					list.Add(i);
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				dataTable.Columns.RemoveAt(list[num]);
			}
			CreateTempTable(dataTable, Temp_Penalization);
			BulkCopy(dataTable, Temp_Penalization);
			CreateTempTable(dtS.Tables["dtDoc"], Temp_Doc);
			BulkCopy(dtS.Tables["dtDoc"], Temp_Doc);
			DataTable dataTable2 = ExecuteDataTable("APZ_SavePenalization", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0][1])))
				{
					objId = Function.ToString(dataTable2.Rows[0][0]);
					objNum = objId;
					ret.Message = "";
				}
				else
				{
					ret.Message = Function.ToString(dataTable2.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable2.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
			}
			else
			{
				ret.Message = "Save Error!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDelete(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocNum", Function.ParseInt(objId)));
			int num = ExecuteNonQuery("apz_Penalization_Remove", CommandType.StoredProcedure, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}
}
