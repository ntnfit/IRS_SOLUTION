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
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class JobVacancyRepository : BaseService<DataSet>, IJobVacancy
{
	private const string Doctemp = "#APZ_OHJV";

	private const string Linetemp1 = "#APZ_HJV7";

	private const string Linetemp2 = "#APZ_HJV1";

	private const string Linetemp3 = "#APZ_HJV2";

	private const string Linetemp4 = "#APZ_HJV3";

	private const string Linetemp5 = "#APZ_HJV4";

	private const string Linetemp6 = "#APZ_HJV5";

	private const string Linetemp7 = "#APZ_HJV6";

	private const string Linetemp8 = "#EmpData";

	private const string Linetemp9 = "#APZ_HJV8";

	public JobVacancyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData(string status, int docNo, string fromdate, string todate)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(fromdate, "yyyy-MM-dd");
			DateTime? dateTime2 = Function.ParseDateTime(todate, "yyyy-MM-dd");
			return ExecuteDataTable("APZ_JobVacancy_GetData", CommandType.StoredProcedure, new SqlParameter("@Status", status), new SqlParameter("@DocNo", docNo), new SqlParameter("@FromDate", dateTime), new SqlParameter("@ToDate", dateTime2));
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
			string query = "APZ_JobVacancy_FindNextLastFrist";
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
			return Process(ApzObjectTypes.oHumanresourcesJobVacacy, -1, transtype, ds);
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
			if (ds == null || ds.Tables.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable dataTable = ds.Tables[0];
			DataTable tableStruct = GetTableStruct("APZ_OHJV");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_OHJV");
			BulkCopy(dataTable, "#APZ_OHJV");
			DataTable dataTable2 = ds.Tables[1];
			DataTable tableStruct2 = GetTableStruct("APZ_HJV7");
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#APZ_HJV7");
			BulkCopy(dataTable2, "#APZ_HJV7");
			DataTable dataTable3 = ds.Tables[2];
			DataTable tableStruct3 = GetTableStruct("APZ_HJV1");
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable3, "#APZ_HJV1");
			BulkCopy(dataTable3, "#APZ_HJV1");
			DataTable dataTable4 = ds.Tables[3];
			DataTable tableStruct4 = GetTableStruct("APZ_HJV2");
			if (!tableStruct4.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source4 = from DataColumn t in tableStruct4.Columns
				select t.ColumnName;
			dataTable4 = dataTable4.DefaultView.ToTable(distinct: false, source4.ToArray());
			CreateTempTable(dataTable4, "#APZ_HJV2");
			BulkCopy(dataTable4, "#APZ_HJV2");
			DataTable dataTable5 = ds.Tables[4];
			DataTable tableStruct5 = GetTableStruct("APZ_HJV3");
			if (!tableStruct5.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source5 = from DataColumn t in tableStruct5.Columns
				select t.ColumnName;
			dataTable5 = dataTable5.DefaultView.ToTable(distinct: false, source5.ToArray());
			CreateTempTable(dataTable5, "#APZ_HJV3");
			BulkCopy(dataTable5, "#APZ_HJV3");
			DataTable dataTable6 = ds.Tables[5];
			DataTable tableStruct6 = GetTableStruct("APZ_HJV4");
			if (!tableStruct6.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source6 = from DataColumn t in tableStruct6.Columns
				select t.ColumnName;
			dataTable6 = dataTable6.DefaultView.ToTable(distinct: false, source6.ToArray());
			CreateTempTable(dataTable6, "#APZ_HJV4");
			BulkCopy(dataTable6, "#APZ_HJV4");
			DataTable dataTable7 = ds.Tables[6];
			DataTable tableStruct7 = GetTableStruct("APZ_HJV5");
			if (!tableStruct7.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source7 = from DataColumn t in tableStruct7.Columns
				select t.ColumnName;
			dataTable7 = dataTable7.DefaultView.ToTable(distinct: false, source7.ToArray());
			CreateTempTable(dataTable7, "#APZ_HJV5");
			BulkCopy(dataTable7, "#APZ_HJV5");
			DataTable dataTable8 = ds.Tables[7];
			DataTable tableStruct8 = GetTableStruct("APZ_HJV6");
			if (!tableStruct8.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source8 = from DataColumn t in tableStruct8.Columns
				select t.ColumnName;
			dataTable8 = dataTable8.DefaultView.ToTable(distinct: false, source8.ToArray());
			CreateTempTable(dataTable8, "#APZ_HJV6");
			BulkCopy(dataTable8, "#APZ_HJV6");
			DataTable dataTable9 = ds.Tables[8];
			DataTable tableStruct9 = GetTableStruct("APZ_HJV8");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source9 = from DataColumn t in tableStruct9.Columns
				select t.ColumnName;
			dataTable9 = dataTable9.DefaultView.ToTable(distinct: false, source9.ToArray());
			CreateTempTable(dataTable9, "#APZ_HJV8");
			BulkCopy(dataTable9, "#APZ_HJV8");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable10 = ExecuteDataTable("APZ_JobVacancy_Saves", CommandType.StoredProcedure, sqlParameter);
			if (dataTable10 != null && dataTable10.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable10.Rows[0][1])))
				{
					objId = Function.ToString(dataTable10.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable10.Rows[0][0])}-{Function.ToString(dataTable10.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Data is not valid" : "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetNo()
	{
		try
		{
			return ExecuteDataTable("apz_JobVacancy_GetNo", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetDetail(int docId)
	{
		try
		{
			string query = "apz_JobVacancy_GetDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docId", docId));
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable SetDataLookUp(string db)
	{
		try
		{
			string query = "select \"Id\",\"Name\" from " + db + ";";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable ProfileMacthUp(string qualification)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Qualifications", qualification);
			return ExecuteDataTable("APZ_JobVacancy_ProfileMatchUp", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListEmpId()
	{
		try
		{
			return ExecuteDataTable("APZ_JobVacancy_GetListEmpId", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable ListBenefits()
	{
		try
		{
			string query = "Select * from APZ_OBNF;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string DeleteDocs(string ids)
	{
		try
		{
			return Process(ApzObjectTypes.oHumanresourcesJobVacacy, -1, Enums.TransactionType.D, new DataSet(), ids).Message;
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
			if (ExecuteNonQuery("APZ_Javacancy_DeleteDocs", CommandType.StoredProcedure, sqlParameter) > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "", "");
			}
			return new HttpResult(Enums.MessageCode.Error, "", "Delete Error!");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public int GetMaxDocNum()
	{
		try
		{
			string query = "SELECT Max(\"DocNum\")  FROM APZ_OHJV;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxDocNum FAILED ", ex.Message);
			return 0;
		}
	}

	public DataTable GetQuaById(int id)
	{
		try
		{
			return ExecuteDataTable("APZ_JobVacancy_GetQuaById", CommandType.StoredProcedure, new SqlParameter("@id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetQuaById FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetEmpById(int id)
	{
		try
		{
			return ExecuteDataTable("APZ_JobVacancy_GetEmpById", CommandType.StoredProcedure, new SqlParameter("@id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetEmpById FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAppById(string id)
	{
		try
		{
			return ExecuteDataTable("APZ_JobVacancy_GetAppById", CommandType.StoredProcedure, new SqlParameter("@id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAppById FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public int GetDocMaxEntry()
	{
		try
		{
			string query = "select MAX(\"DocEntry\") from APZ_OHJV;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocEntry FAILED ", ex.Message);
			return 0;
		}
	}

	public int GetNextNumber(int series)
	{
		try
		{
			string query = "select \"NextNumber\" from nnm1  where \"Series\"=@Series;";
			SqlParameter sqlParameter = new SqlParameter("@Series", series);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 1;
		}
	}
}
