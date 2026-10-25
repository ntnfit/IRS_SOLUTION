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
using Apzon.Services.Interfaces.Administrator.Setup.Purchasing;

namespace Apzon.Services.Repository.Administrator.Setup.Purchasing;

public class LandedCostsRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), ILandedCosts
{
	private string TempDoc = "#OIPF";

	private const string Linetemp1 = "#IPF1";

	private const string Linetemp2 = "#IPF2";

	private const string Linetemp3 = "#IPF3";

	private const string Linetemp4 = "#ATC1";

	public DataTable SearchData(string fromDate, string toDate, string vendor, string broker, int docNum)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(fromDate, "ddMMyyyy");
			DateTime? dateTime2 = Function.ParseDateTime(toDate, "ddMMyyyy");
			string query = "APZ_LandedCosts_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@fromDate", dateTime));
			list.Add(new SqlParameter("@toDate", dateTime2));
			list.Add(new SqlParameter("@vendor", vendor));
			list.Add(new SqlParameter("@broker", broker));
			list.Add(new SqlParameter("@docNum", docNum));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetDetaiByDocId(int docId)
	{
		try
		{
			string query = "APZ_LanedCost_GetDetail";
			return ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@docId", docId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable FindDataNextLast(int id, int type)
	{
		try
		{
			string query = "APZ_LandedCosts_FindNextLastFrist";
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

	public DataTable GetNo()
	{
		try
		{
			string query = "select \"Series\",\"SeriesName\" from nnm1 where \"ObjectCode\"='69';";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetPriceList()
	{
		try
		{
			string query = "select \"ListNum\",\"ListName\" from OPLN;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetLandedCostName()
	{
		try
		{
			return ExecuteDataTable("APZ_LandedCosts_GetNameLandedCosts", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
			return Process(ApzObjectTypes.oPurchaseLandedCost, -1, transtype, ds);
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
			decimal num = ds.Tables[1].AsEnumerable().Sum((DataRow t) => Function.ParseDecimal(t["TtlExpndLC"]));
			decimal num2 = ds.Tables[2].AsEnumerable().Sum((DataRow t) => Function.ParseDecimal(t["CostSum"]));
			if (num > num2 || num < num2)
			{
				return new HttpResult(Enums.MessageCode.Error, "Sum of rows does not match totals!");
			}
			DataTable dataTable = ds.Tables[0];
			DataTable tableStruct = GetTableStruct("OIPF");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, TempDoc);
			BulkCopy(dataTable, TempDoc);
			DataTable dataTable2 = ds.Tables[1];
			DataTable tableStruct2 = GetTableStruct("IPF1");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#IPF1");
			BulkCopy(dataTable2, "#IPF1");
			DataTable dataTable3 = ds.Tables[2];
			DataTable tableStruct3 = GetTableStruct("IPF2");
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable3, "#IPF2");
			BulkCopy(dataTable3, "#IPF2");
			DataTable dataTable4 = ds.Tables[3];
			DataTable tableStruct4 = GetTableStruct("IPF1");
			if (!tableStruct4.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source4 = from DataColumn t in tableStruct4.Columns
				select t.ColumnName;
			dataTable4 = dataTable4.DefaultView.ToTable(distinct: false, source4.ToArray());
			CreateTempTable(dataTable4, "#IPF3");
			BulkCopy(dataTable4, "#IPF3");
			DataTable dataTable5 = ds.Tables[4];
			DataTable tableStruct5 = GetTableStruct("ATC1");
			if (!tableStruct5.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source5 = from DataColumn t in tableStruct5.Columns
				select t.ColumnName;
			dataTable5 = dataTable5.DefaultView.ToTable(distinct: false, source5.ToArray());
			CreateTempTable(dataTable5, "#ATC1");
			BulkCopy(dataTable5, "#ATC1");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable6 = ExecuteDataTable("APZ_LandedCost_Saves", CommandType.StoredProcedure, sqlParameter);
			if (dataTable6 != null && dataTable6.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable6.Rows[0][1])))
				{
					objId = Function.ToString(dataTable6.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable6.Rows[0][0])}-{Function.ToString(dataTable6.Rows[0][1])}");
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
			return Process(ApzObjectTypes.oPurchaseLandedCost, -1, Enums.TransactionType.D, new DataSet()).Message;
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
			DataTable dataTable = ExecuteDataTable("APZ_LandedCosts_DeleteDocs", CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new HttpResult(Enums.MessageCode.Success, "", "Delete success!");
			}
			return new HttpResult(Enums.MessageCode.Success, "", "Delete Error!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetListDetailGLI(int objectType, string ids)
	{
		try
		{
			string query = "APZ_LandedCost_GetDetailGLI";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@objectType", objectType));
			list.Add(new SqlParameter("@ids", ids));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBusinessPartner(string cardCode, string cardType, string userName, string branch)
	{
		try
		{
			string query = "Apz_LandedCost_GetBusinessPartner";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@cardCode", cardCode));
			list.Add(new SqlParameter("@cardType", cardType));
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@branch", branch));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetMaxDocEntryQuery()
	{
		return "SELECT ISNULL(MAX(DocEntry), 0) as maxCode from OIPF";
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string maxDocEntryQuery = GetMaxDocEntryQuery();
			DataTable dataTable = ExecuteDataTable(maxDocEntryQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["maxCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMax DocEntry  FAILED ", ex.Message);
			return 0;
		}
	}
}
