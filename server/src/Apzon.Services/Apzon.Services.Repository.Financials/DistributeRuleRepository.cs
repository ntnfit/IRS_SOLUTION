using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Financials;

namespace Apzon.Services.Repository.Financials;

public class DistributeRuleRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IDistributeRule
{
	private string tempDistributeName = "#APZ_TEMP_DISTRIBUTE_RULE";

	private string tempDistributeLineName = "#APZ_TEMP_DISTRIBUTE_RULE_LINE";

	public DataTable GetOOCR()
	{
		try
		{
			string query = "APZ_GetDocDitributeRule";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetOCR1(string key)
	{
		try
		{
			string query = "APZ_GetLineDitributeRule";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@key", key));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataLine  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string Save(DataSet ds)
	{
		try
		{
			bool flag = CheckDoc(Function.ToString(ds.Tables[0].Rows[0]["OcrCode"]));
			return Process(ApzObjectTypes.oDistributionRule, -1, flag ? Enums.TransactionType.U : Enums.TransactionType.A, ds).Message;
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
			ProcessDistributeRule(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessDistributeRule(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDeleteDistributeRule(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessDeleteDistributeRule(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_process_DistributeRule_Remove", CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				ret.Message = "Update error: " + ret.Message;
			}
			else
			{
				ret.Message = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDistributeRule(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables.Count != 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save (DataSet is null Or table is null)";
			}
			else
			{
				string text = ((transtype == Enums.TransactionType.A) ? "A" : "U");
				CreateTempTable(ds.Tables[0], tempDistributeName);
				BulkCopy(ds.Tables[0], tempDistributeName);
				CreateTempTable(ds.Tables[1], tempDistributeLineName);
				BulkCopy(ds.Tables[1], tempDistributeLineName);
				SqlParameter sqlParameter = new SqlParameter("@Type", text);
				DataTable dataTable = ExecuteDataTable("apz_process_DistributeRule", CommandType.StoredProcedure, sqlParameter);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					if (text == "A")
					{
						ret.Message = "Save error: " + ret.Message;
					}
					else
					{
						ret.Message = "Update error: " + ret.Message;
					}
				}
				else
				{
					ret.Message = "";
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(ds.Tables[0].Rows[0]["OcrCode"]);
				objNum = Function.ToString(ds.Tables[0].Rows[0]["OcrCode"]);
			}
			else
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public bool CheckDoc(string ocrCode)
	{
		try
		{
			string query = "SELECT 1\r\n                            FROM OOCR \r\n                            WHERE ocrCode = @ocrCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ocrCode", ocrCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public DataTable Search(DataTable dtSearch)
	{
		try
		{
			string text = Function.ToString(dtSearch.Rows[0]["OcrCode"]);
			string text2 = Function.ToString(dtSearch.Rows[0]["OcrName"]);
			int num = Function.ParseInt(dtSearch.Rows[0]["DimCode"]);
			List<SqlParameter> list = new List<SqlParameter>();
			string query = SearchQuery(text, text2, num);
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(new SqlParameter("@OcrCode", text));
			}
			if (!string.IsNullOrEmpty(text2))
			{
				list.Add(new SqlParameter("@OcrName", text2));
			}
			if (num > 0)
			{
				list.Add(new SqlParameter("@DimCode", num));
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet LoadComBoBox()
	{
		DataSet dataSet = new DataSet();
		try
		{
			DataTable dataTable = LoadComBoDimension();
			DataSet dataSet2 = dataTable.DataSet;
			if (dataSet2 != null)
			{
				dataSet2.Tables.Clear();
				dataSet2.AcceptChanges();
			}
			DataTable dataTable2 = LoadComBoGeneralCostCenter();
			DataSet dataSet3 = dataTable2.DataSet;
			if (dataSet3 != null)
			{
				dataSet3.Tables.Clear();
				dataSet3.AcceptChanges();
			}
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return dataSet;
	}

	public DataTable LoadComBoDimension()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"DimCode\",\"DimDesc\" from ODIM where \"DimActive\"='Y';";
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable LoadComBoGeneralCostCenter()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"PrcCode\", \"PrcName\", \"DimCode\" from oprc where UPPER(LEFT(\"PrcCode\",7)) = 'CENTR_Z';";
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string DeleteDistributeRule(string OcrCode)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_DeleteDistributeRule";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@OcrCode", OcrCode));
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
				dbTransaction.Commit();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable LoadPrcCode()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"PrcCode\" from OPRC;";
			result = ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public bool GetIsHasLinkedObject(string key)
	{
		try
		{
			string query = "Select \"DimCode\" from OOCR where \"OcrCode\"=@key;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@key", key));
			int num = 0;
			if (dataTable.IsNotNull())
			{
				num = Function.ParseInt(dataTable.Rows[0][0]);
				query = GetSQLCheckExists(num, key);
				DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@key", key));
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public DataTable GetStruct()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM OOCR;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	private string GetSQLCheckExists(int dimension, string key)
	{
		try
		{
			string[,] array = new string[1021, 3]
			{
				{ "CPI3", "OcrCode", "1" },
				{ "CPI3", "OcrCode2", "2" },
				{ "CPI3", "OcrCode3", "3" },
				{ "CPI3", "OcrCode4", "4" },
				{ "CPI3", "OcrCode5", "5" },
				{ "CPI2", "OcrCode", "1" },
				{ "CPI2", "OcrCode2", "2" },
				{ "CPI2", "OcrCode3", "3" },
				{ "CPI2", "OcrCode4", "4" },
				{ "CPI2", "OcrCode5", "5" },
				{ "CPI1", "OcrCode", "1" },
				{ "CPI1", "OcrCode2", "2" },
				{ "CPI1", "OcrCode3", "3" },
				{ "CPI1", "OcrCode4", "4" },
				{ "CPI1", "OcrCode5", "5" },
				{ "CPI1", "CogsOcrCod", "1" },
				{ "CPI1", "CogsOcrCo2", "2" },
				{ "CPI1", "CogsOcrCo3", "3" },
				{ "CPI1", "CogsOcrCo4", "4" },
				{ "CPI1", "CogsOcrCo5", "5" },
				{ "CPV3", "OcrCode", "1" },
				{ "CPV3", "OcrCode2", "2" },
				{ "CPV3", "OcrCode3", "3" },
				{ "CPV3", "OcrCode4", "4" },
				{ "CPV3", "OcrCode5", "5" },
				{ "CPV2", "OcrCode", "1" },
				{ "CPV2", "OcrCode2", "2" },
				{ "CPV2", "OcrCode3", "3" },
				{ "CPV2", "OcrCode4", "4" },
				{ "CPV2", "OcrCode5", "5" },
				{ "CPV1", "OcrCode", "1" },
				{ "CPV1", "OcrCode2", "2" },
				{ "CPV1", "OcrCode3", "3" },
				{ "CPV1", "OcrCode4", "4" },
				{ "CPV1", "OcrCode5", "5" },
				{ "CPV1", "CogsOcrCod", "1" },
				{ "CPV1", "CogsOcrCo2", "2" },
				{ "CPV1", "CogsOcrCo3", "3" },
				{ "CPV1", "CogsOcrCo4", "4" },
				{ "CPV1", "CogsOcrCo5", "5" },
				{ "CPV13", "OcrCode", "1" },
				{ "CPV13", "OcrCode2", "2" },
				{ "CPV13", "OcrCode3", "3" },
				{ "CPV13", "OcrCode4", "4" },
				{ "CPV13", "OcrCode5", "5" },
				{ "CPI13", "OcrCode", "1" },
				{ "CPI13", "OcrCode2", "2" },
				{ "CPI13", "OcrCode3", "3" },
				{ "CPI13", "OcrCode4", "4" },
				{ "CPI13", "OcrCode5", "5" },
				{ "RPC3", "OcrCode", "1" },
				{ "RPC3", "OcrCode2", "2" },
				{ "RPC3", "OcrCode3", "3" },
				{ "RPC3", "OcrCode4", "4" },
				{ "RPC3", "OcrCode5", "5" },
				{ "RPC1", "OcrCode", "1" },
				{ "RPC1", "OcrCode2", "2" },
				{ "RPC1", "OcrCode3", "3" },
				{ "RPC1", "OcrCode4", "4" },
				{ "RPC1", "OcrCode5", "5" },
				{ "RPC1", "CogsOcrCod", "1" },
				{ "RPC1", "CogsOcrCo2", "2" },
				{ "RPC1", "CogsOcrCo3", "3" },
				{ "RPC1", "CogsOcrCo4", "4" },
				{ "RPC1", "CogsOcrCo5", "5" },
				{ "RPC13", "OcrCode", "1" },
				{ "RPC13", "OcrCode2", "2" },
				{ "RPC13", "OcrCode3", "3" },
				{ "RPC13", "OcrCode4", "4" },
				{ "RPC13", "OcrCode5", "5" },
				{ "RPC2", "OcrCode", "1" },
				{ "RPC2", "OcrCode2", "2" },
				{ "RPC2", "OcrCode3", "3" },
				{ "RPC2", "OcrCode4", "4" },
				{ "RPC2", "OcrCode5", "5" },
				{ "DPO3", "OcrCode", "1" },
				{ "DPO3", "OcrCode2", "2" },
				{ "DPO3", "OcrCode3", "3" },
				{ "DPO3", "OcrCode4", "4" },
				{ "DPO3", "OcrCode5", "5" },
				{ "DPO2", "OcrCode", "1" },
				{ "DPO2", "OcrCode2", "2" },
				{ "DPO2", "OcrCode3", "3" },
				{ "DPO2", "OcrCode4", "4" },
				{ "DPO2", "OcrCode5", "5" },
				{ "DPO1", "OcrCode", "1" },
				{ "DPO1", "OcrCode2", "2" },
				{ "DPO1", "OcrCode3", "3" },
				{ "DPO1", "OcrCode4", "4" },
				{ "DPO1", "OcrCode5", "5" },
				{ "DPO1", "CogsOcrCod", "1" },
				{ "DPO1", "CogsOcrCo2", "2" },
				{ "DPO1", "CogsOcrCo3", "3" },
				{ "DPO1", "CogsOcrCo4", "4" },
				{ "DPO1", "CogsOcrCo5", "5" },
				{ "DPO13", "OcrCode", "1" },
				{ "DPO13", "OcrCode2", "2" },
				{ "DPO13", "OcrCode3", "3" },
				{ "DPO13", "OcrCode4", "4" },
				{ "DPO13", "OcrCode5", "5" },
				{ "PCH3", "OcrCode", "1" },
				{ "PCH3", "OcrCode2", "2" },
				{ "PCH3", "OcrCode3", "3" },
				{ "PCH3", "OcrCode4", "4" },
				{ "PCH3", "OcrCode5", "5" },
				{ "PCH2", "OcrCode", "1" },
				{ "PCH2", "OcrCode2", "2" },
				{ "PCH2", "OcrCode3", "3" },
				{ "PCH2", "OcrCode4", "4" },
				{ "PCH2", "OcrCode5", "5" },
				{ "PCH1", "OcrCode", "1" },
				{ "PCH1", "OcrCode2", "2" },
				{ "PCH1", "OcrCode3", "3" },
				{ "PCH1", "OcrCode4", "4" },
				{ "PCH1", "OcrCode5", "5" },
				{ "PCH1", "CogsOcrCod", "1" },
				{ "PCH1", "CogsOcrCo2", "2" },
				{ "PCH1", "CogsOcrCo3", "3" },
				{ "PCH1", "CogsOcrCo4", "4" },
				{ "PCH1", "CogsOcrCo5", "5" },
				{ "PCH13", "OcrCode", "1" },
				{ "PCH13", "OcrCode2", "2" },
				{ "PCH13", "OcrCode3", "3" },
				{ "PCH13", "OcrCode4", "4" },
				{ "PCH13", "OcrCode5", "5" },
				{ "CSI2", "OcrCode", "1" },
				{ "CSI2", "OcrCode2", "2" },
				{ "CSI2", "OcrCode3", "3" },
				{ "CSI2", "OcrCode4", "4" },
				{ "CSI2", "OcrCode5", "5" },
				{ "CIN3", "OcrCode", "1" },
				{ "CIN3", "OcrCode2", "2" },
				{ "CIN3", "OcrCode3", "3" },
				{ "CIN3", "OcrCode4", "4" },
				{ "CIN3", "OcrCode5", "5" },
				{ "CSI3", "OcrCode", "1" },
				{ "CSI3", "OcrCode2", "2" },
				{ "CSI3", "OcrCode3", "3" },
				{ "CSI3", "OcrCode4", "4" },
				{ "CSI3", "OcrCode5", "5" },
				{ "CIN2", "OcrCode", "1" },
				{ "CIN2", "OcrCode2", "2" },
				{ "CIN2", "OcrCode3", "3" },
				{ "CIN2", "OcrCode4", "4" },
				{ "CIN2", "OcrCode5", "5" },
				{ "CSI1", "OcrCode", "1" },
				{ "CSI1", "OcrCode2", "2" },
				{ "CSI1", "OcrCode3", "3" },
				{ "CSI1", "OcrCode4", "4" },
				{ "CSI1", "OcrCode5", "5" },
				{ "CSI1", "CogsOcrCod", "1" },
				{ "CSI1", "CogsOcrCo2", "2" },
				{ "CSI1", "CogsOcrCo3", "3" },
				{ "CSI1", "CogsOcrCo4", "4" },
				{ "CSI1", "CogsOcrCo5", "5" },
				{ "CSV3", "OcrCode", "1" },
				{ "CSV3", "OcrCode2", "2" },
				{ "CSV3", "OcrCode3", "3" },
				{ "CSV3", "OcrCode4", "4" },
				{ "CSV3", "OcrCode5", "5" },
				{ "CSV2", "OcrCode", "1" },
				{ "CSV2", "OcrCode2", "2" },
				{ "CSV2", "OcrCode3", "3" },
				{ "CSV2", "OcrCode4", "4" },
				{ "CSV2", "OcrCode5", "5" },
				{ "CSV1", "OcrCode", "1" },
				{ "CSV1", "OcrCode2", "2" },
				{ "CSV1", "OcrCode3", "3" },
				{ "CSV1", "OcrCode4", "4" },
				{ "CSV1", "OcrCode5", "5" },
				{ "CSV1", "CogsOcrCod", "1" },
				{ "CSV1", "CogsOcrCo2", "2" },
				{ "CSV1", "CogsOcrCo3", "3" },
				{ "CSV1", "CogsOcrCo4", "4" },
				{ "CSV1", "CogsOcrCo5", "5" },
				{ "CSV13", "OcrCode", "1" },
				{ "CSV13", "OcrCode2", "2" },
				{ "CSV13", "OcrCode3", "3" },
				{ "CSV13", "OcrCode4", "4" },
				{ "CSV13", "OcrCode5", "5" },
				{ "CSI13", "OcrCode", "1" },
				{ "CSI13", "OcrCode2", "2" },
				{ "CSI13", "OcrCode3", "3" },
				{ "CSI13", "OcrCode4", "4" },
				{ "CSI13", "OcrCode5", "5" },
				{ "RIN3", "OcrCode", "1" },
				{ "RIN3", "OcrCode2", "2" },
				{ "RIN3", "OcrCode3", "3" },
				{ "RIN3", "OcrCode4", "4" },
				{ "RIN3", "OcrCode5", "5" },
				{ "RIN2", "OcrCode", "1" },
				{ "RIN2", "OcrCode2", "2" },
				{ "RIN2", "OcrCode3", "3" },
				{ "RIN2", "OcrCode4", "4" },
				{ "RIN2", "OcrCode5", "5" },
				{ "RIN1", "OcrCode", "1" },
				{ "RIN1", "OcrCode2", "2" },
				{ "RIN1", "OcrCode3", "3" },
				{ "RIN1", "OcrCode4", "4" },
				{ "RIN1", "OcrCode5", "5" },
				{ "RIN1", "CogsOcrCod", "1" },
				{ "RIN1", "CogsOcrCo2", "2" },
				{ "RIN1", "CogsOcrCo3", "3" },
				{ "RIN1", "CogsOcrCo4", "4" },
				{ "RIN1", "CogsOcrCo5", "5" },
				{ "RIN13", "OcrCode", "1" },
				{ "RIN13", "OcrCode2", "2" },
				{ "RIN13", "OcrCode3", "3" },
				{ "RIN13", "OcrCode4", "4" },
				{ "RIN13", "OcrCode5", "5" },
				{ "DPI3", "OcrCode", "1" },
				{ "DPI3", "OcrCode2", "2" },
				{ "DPI3", "OcrCode3", "3" },
				{ "DPI3", "OcrCode4", "4" },
				{ "DPI3", "OcrCode5", "5" },
				{ "DPI2", "OcrCode", "1" },
				{ "DPI2", "OcrCode2", "2" },
				{ "DPI2", "OcrCode3", "3" },
				{ "DPI2", "OcrCode4", "4" },
				{ "DPI2", "OcrCode5", "5" },
				{ "DPI1", "OcrCode", "1" },
				{ "DPI1", "OcrCode2", "2" },
				{ "DPI1", "OcrCode3", "3" },
				{ "DPI1", "OcrCode4", "4" },
				{ "DPI1", "OcrCode5", "5" },
				{ "DPI1", "CogsOcrCod", "1" },
				{ "DPI1", "CogsOcrCo2", "2" },
				{ "DPI1", "CogsOcrCo3", "3" },
				{ "DPI1", "CogsOcrCo4", "4" },
				{ "DPI1", "CogsOcrCo5", "5" },
				{ "DPI13", "OcrCode", "1" },
				{ "DPI13", "OcrCode2", "2" },
				{ "DPI13", "OcrCode3", "3" },
				{ "DPI13", "OcrCode4", "4" },
				{ "DPI13", "OcrCode5", "5" },
				{ "INV3", "OcrCode", "1" },
				{ "INV3", "OcrCode2", "2" },
				{ "INV3", "OcrCode3", "3" },
				{ "INV3", "OcrCode4", "4" },
				{ "INV3", "OcrCode5", "5" },
				{ "ADO3", "OcrCode", "1" },
				{ "ADO3", "OcrCode2", "2" },
				{ "ADO3", "OcrCode3", "3" },
				{ "ADO3", "OcrCode4", "4" },
				{ "ADO3", "OcrCode5", "5" },
				{ "ADO2", "OcrCode", "1" },
				{ "ADO2", "OcrCode2", "2" },
				{ "ADO2", "OcrCode3", "3" },
				{ "ADO2", "OcrCode4", "4" },
				{ "ADO2", "OcrCode5", "5" },
				{ "INV2", "OcrCode", "1" },
				{ "INV2", "OcrCode2", "2" },
				{ "INV2", "OcrCode3", "3" },
				{ "INV2", "OcrCode4", "4" },
				{ "INV2", "OcrCode5", "5" },
				{ "INV1", "OcrCode", "1" },
				{ "INV1", "OcrCode2", "2" },
				{ "INV1", "OcrCode3", "3" },
				{ "INV1", "OcrCode4", "4" },
				{ "INV1", "OcrCode5", "5" },
				{ "INV1", "CogsOcrCod", "1" },
				{ "INV1", "CogsOcrCo2", "2" },
				{ "INV1", "CogsOcrCo3", "3" },
				{ "INV1", "CogsOcrCo4", "4" },
				{ "INV1", "CogsOcrCo5", "5" },
				{ "ADO1", "OcrCode", "1" },
				{ "ADO1", "OcrCode2", "2" },
				{ "ADO1", "OcrCode3", "3" },
				{ "ADO1", "OcrCode4", "4" },
				{ "ADO1", "OcrCode5", "5" },
				{ "ADO1", "CogsOcrCod", "1" },
				{ "ADO1", "CogsOcrCo2", "2" },
				{ "ADO1", "CogsOcrCo3", "3" },
				{ "ADO1", "CogsOcrCo4", "4" },
				{ "ADO1", "CogsOcrCo5", "5" },
				{ "ADO13", "OcrCode", "1" },
				{ "ADO13", "OcrCode2", "2" },
				{ "ADO13", "OcrCode3", "3" },
				{ "ADO13", "OcrCode4", "4" },
				{ "ADO13", "OcrCode5", "5" },
				{ "INV13", "OcrCode", "1" },
				{ "INV13", "OcrCode2", "2" },
				{ "INV13", "OcrCode3", "3" },
				{ "INV13", "OcrCode4", "4" },
				{ "INV13", "OcrCode5", "5" },
				{ "OAIM", "OcrCode", "1" },
				{ "OAIM", "OcrCode2", "2" },
				{ "OAIM", "OcrCode3", "3" },
				{ "OAIM", "OcrCode4", "4" },
				{ "OAIM", "OcrCode5", "5" },
				{ "AIT6", "OcrCode", "1" },
				{ "AIT6", "OcrCode2", "2" },
				{ "AIT6", "OcrCode3", "3" },
				{ "AIT6", "OcrCode4", "4" },
				{ "AIT6", "OcrCode5", "5" },
				{ "ITM6", "OcrCode", "1" },
				{ "ITM6", "OcrCode2", "2" },
				{ "ITM6", "OcrCode3", "3" },
				{ "ITM6", "OcrCode4", "4" },
				{ "ITM6", "OcrCode5", "5" },
				{ "BNK2", "PrftCenter", "1" },
				{ "BNK2", "PrftCent2", "2" },
				{ "BNK2", "PrftCent3", "3" },
				{ "BNK2", "PrftCent4", "4" },
				{ "BNK2", "PrftCent5", "5" },
				{ "ITT1", "OcrCode", "1" },
				{ "ITT1", "OcrCode2", "2" },
				{ "ITT1", "OcrCode3", "3" },
				{ "ITT1", "OcrCode4", "4" },
				{ "ITT1", "OcrCode5", "5" },
				{ "ATT1", "OcrCode", "1" },
				{ "ATT1", "OcrCode2", "2" },
				{ "ATT1", "OcrCode3", "3" },
				{ "ATT1", "OcrCode4", "4" },
				{ "ATT1", "OcrCode5", "5" },
				{ "BGT2", "OcrCode", "1" },
				{ "BGT3", "OcrCode", "1" },
				{ "OBGS", "OcrCode", "1" },
				{ "OBGS", "OcrCode2", "2" },
				{ "OBGS", "OcrCode3", "3" },
				{ "OBGS", "OcrCode4", "4" },
				{ "OBGS", "OcrCode5", "5" },
				{ "ACQ1", "OcrCode", "1" },
				{ "ACQ1", "OcrCode2", "2" },
				{ "ACQ1", "OcrCode3", "3" },
				{ "ACQ1", "OcrCode4", "4" },
				{ "ACQ1", "OcrCode5", "5" },
				{ "CIN1", "OcrCode", "1" },
				{ "CIN1", "OcrCode2", "2" },
				{ "CIN1", "OcrCode3", "3" },
				{ "CIN1", "OcrCode4", "4" },
				{ "CIN1", "OcrCode5", "5" },
				{ "CIN1", "CogsOcrCod", "1" },
				{ "CIN1", "CogsOcrCo2", "2" },
				{ "CIN1", "CogsOcrCo3", "3" },
				{ "CIN1", "CogsOcrCo4", "4" },
				{ "CIN1", "CogsOcrCo5", "5" },
				{ "CIN13", "OcrCode", "1" },
				{ "CIN13", "OcrCode2", "2" },
				{ "CIN13", "OcrCode3", "3" },
				{ "CIN13", "OcrCode4", "4" },
				{ "CIN13", "OcrCode5", "5" },
				{ "ACD1", "OcrCode", "1" },
				{ "ACD1", "OcrCode2", "2" },
				{ "ACD1", "OcrCode3", "3" },
				{ "ACD1", "OcrCode4", "4" },
				{ "ACD1", "OcrCode5", "5" },
				{ "DLN1", "OcrCode", "1" },
				{ "DLN1", "OcrCode2", "2" },
				{ "DLN1", "OcrCode3", "3" },
				{ "DLN1", "OcrCode4", "4" },
				{ "DLN1", "OcrCode5", "5" },
				{ "DLN1", "CogsOcrCod", "1" },
				{ "DLN1", "CogsOcrCo2", "2" },
				{ "DLN1", "CogsOcrCo3", "3" },
				{ "DLN1", "CogsOcrCo4", "4" },
				{ "DLN1", "CogsOcrCo5", "5" },
				{ "DLN3", "OcrCode", "1" },
				{ "DLN3", "OcrCode2", "2" },
				{ "DLN3", "OcrCode3", "3" },
				{ "DLN3", "OcrCode4", "4" },
				{ "DLN3", "OcrCode5", "5" },
				{ "DLN2", "OcrCode", "1" },
				{ "DLN2", "OcrCode2", "2" },
				{ "DLN2", "OcrCode3", "3" },
				{ "DLN2", "OcrCode4", "4" },
				{ "DLN2", "OcrCode5", "5" },
				{ "DLN13", "OcrCode", "1" },
				{ "DLN13", "OcrCode2", "2" },
				{ "DLN13", "OcrCode3", "3" },
				{ "DLN13", "OcrCode4", "4" },
				{ "DLN13", "OcrCode5", "5" },
				{ "ODPS", "OcrCode", "1" },
				{ "ODPS", "OcrCode2", "2" },
				{ "ODPS", "OcrCode3", "3" },
				{ "ODPS", "OcrCode4", "4" },
				{ "ODPS", "OcrCode5", "5" },
				{ "DRF3", "OcrCode", "1" },
				{ "DRF3", "OcrCode2", "2" },
				{ "DRF3", "OcrCode3", "3" },
				{ "DRF3", "OcrCode4", "4" },
				{ "DRF3", "OcrCode5", "5" },
				{ "DRF2", "OcrCode", "1" },
				{ "DRF2", "OcrCode2", "2" },
				{ "DRF2", "OcrCode3", "3" },
				{ "DRF2", "OcrCode4", "4" },
				{ "DRF2", "OcrCode5", "5" },
				{ "DRF1", "OcrCode", "1" },
				{ "DRF1", "OcrCode2", "2" },
				{ "DRF1", "OcrCode3", "3" },
				{ "DRF1", "OcrCode4", "4" },
				{ "DRF1", "OcrCode5", "5" },
				{ "DRF1", "CogsOcrCod", "1" },
				{ "DRF1", "CogsOcrCo2", "2" },
				{ "DRF1", "CogsOcrCo3", "3" },
				{ "DRF1", "CogsOcrCo4", "4" },
				{ "DRF1", "CogsOcrCo5", "5" },
				{ "DRF13", "OcrCode", "1" },
				{ "DRF13", "OcrCode2", "2" },
				{ "DRF13", "OcrCode3", "3" },
				{ "DRF13", "OcrCode4", "4" },
				{ "DRF13", "OcrCode5", "5" },
				{ "OBNK", "FeeProfitC", "1" },
				{ "OBNK", "FeeProfit2", "2" },
				{ "OBNK", "FeeProfit3", "3" },
				{ "OBNK", "FeeProfit4", "4" },
				{ "OBNK", "FeeProfit5", "5" },
				{ "AEXD", "OcrCode", "1" },
				{ "AEXD", "OcrCode2", "2" },
				{ "AEXD", "OcrCode3", "3" },
				{ "AEXD", "OcrCode4", "4" },
				{ "AEXD", "OcrCode5", "5" },
				{ "OEXD", "OcrCode", "1" },
				{ "OEXD", "OcrCode2", "2" },
				{ "OEXD", "OcrCode3", "3" },
				{ "OEXD", "OcrCode4", "4" },
				{ "OEXD", "OcrCode5", "5" },
				{ "AACT", "OverCode", "1" },
				{ "AACT", "OverCode2", "2" },
				{ "AACT", "OverCode3", "3" },
				{ "AACT", "OverCode4", "4" },
				{ "AACT", "OverCode5", "5" },
				{ "OACT", "OverCode", "1" },
				{ "OACT", "OverCode2", "2" },
				{ "OACT", "OverCode3", "3" },
				{ "OACT", "OverCode4", "4" },
				{ "OACT", "OverCode5", "5" },
				{ "IGE3", "OcrCode", "1" },
				{ "IGE3", "OcrCode2", "2" },
				{ "IGE3", "OcrCode3", "3" },
				{ "IGE3", "OcrCode4", "4" },
				{ "IGE3", "OcrCode5", "5" },
				{ "IGE2", "OcrCode", "1" },
				{ "IGE2", "OcrCode2", "2" },
				{ "IGE2", "OcrCode3", "3" },
				{ "IGE2", "OcrCode4", "4" },
				{ "IGE2", "OcrCode5", "5" },
				{ "IGE1", "OcrCode", "1" },
				{ "IGE1", "OcrCode2", "2" },
				{ "IGE1", "OcrCode3", "3" },
				{ "IGE1", "OcrCode4", "4" },
				{ "IGE1", "OcrCode5", "5" },
				{ "IGE1", "CogsOcrCod", "1" },
				{ "IGE1", "CogsOcrCo2", "2" },
				{ "IGE1", "CogsOcrCo3", "3" },
				{ "IGE1", "CogsOcrCo4", "4" },
				{ "IGE1", "CogsOcrCo5", "5" },
				{ "IGE13", "OcrCode", "1" },
				{ "IGE13", "OcrCode2", "2" },
				{ "IGE13", "OcrCode3", "3" },
				{ "IGE13", "OcrCode4", "4" },
				{ "IGE13", "OcrCode5", "5" },
				{ "IGN3", "OcrCode", "1" },
				{ "IGN3", "OcrCode2", "2" },
				{ "IGN3", "OcrCode3", "3" },
				{ "IGN3", "OcrCode4", "4" },
				{ "IGN3", "OcrCode5", "5" },
				{ "IGN2", "OcrCode", "1" },
				{ "IGN2", "OcrCode2", "2" },
				{ "IGN2", "OcrCode3", "3" },
				{ "IGN2", "OcrCode4", "4" },
				{ "IGN2", "OcrCode5", "5" },
				{ "IGN1", "OcrCode", "1" },
				{ "IGN1", "OcrCode2", "2" },
				{ "IGN1", "OcrCode3", "3" },
				{ "IGN1", "OcrCode4", "4" },
				{ "IGN1", "OcrCode5", "5" },
				{ "IGN1", "CogsOcrCod", "1" },
				{ "IGN1", "CogsOcrCo2", "2" },
				{ "IGN1", "CogsOcrCo3", "3" },
				{ "IGN1", "CogsOcrCo4", "4" },
				{ "IGN1", "CogsOcrCo5", "5" },
				{ "IGN13", "OcrCode", "1" },
				{ "IGN13", "OcrCode2", "2" },
				{ "IGN13", "OcrCode3", "3" },
				{ "IGN13", "OcrCode4", "4" },
				{ "IGN13", "OcrCode5", "5" },
				{ "PDN3", "OcrCode", "1" },
				{ "PDN3", "OcrCode2", "2" },
				{ "PDN3", "OcrCode3", "3" },
				{ "PDN3", "OcrCode4", "4" },
				{ "PDN3", "OcrCode5", "5" },
				{ "PDN2", "OcrCode", "1" },
				{ "PDN2", "OcrCode2", "2" },
				{ "PDN2", "OcrCode3", "3" },
				{ "PDN2", "OcrCode4", "4" },
				{ "PDN2", "OcrCode5", "5" },
				{ "PDN1", "OcrCode", "1" },
				{ "PDN1", "OcrCode2", "2" },
				{ "PDN1", "OcrCode3", "3" },
				{ "PDN1", "OcrCode4", "4" },
				{ "PDN1", "OcrCode5", "5" },
				{ "PDN1", "CogsOcrCod", "1" },
				{ "PDN1", "CogsOcrCo2", "2" },
				{ "PDN1", "CogsOcrCo3", "3" },
				{ "PDN1", "CogsOcrCo4", "4" },
				{ "PDN1", "CogsOcrCo5", "5" },
				{ "PDN13", "OcrCode", "1" },
				{ "PDN13", "OcrCode2", "2" },
				{ "PDN13", "OcrCode3", "3" },
				{ "PDN13", "OcrCode4", "4" },
				{ "PDN13", "OcrCode5", "5" },
				{ "RPD3", "OcrCode", "1" },
				{ "RPD3", "OcrCode2", "2" },
				{ "RPD3", "OcrCode3", "3" },
				{ "RPD3", "OcrCode4", "4" },
				{ "RPD3", "OcrCode5", "5" },
				{ "RPD2", "OcrCode", "1" },
				{ "RPD2", "OcrCode2", "2" },
				{ "RPD2", "OcrCode3", "3" },
				{ "RPD2", "OcrCode4", "4" },
				{ "RPD2", "OcrCode5", "5" },
				{ "RPD1", "OcrCode", "1" },
				{ "RPD1", "OcrCode2", "2" },
				{ "RPD1", "OcrCode3", "3" },
				{ "RPD1", "OcrCode4", "4" },
				{ "RPD1", "OcrCode5", "5" },
				{ "RPD1", "CogsOcrCod", "1" },
				{ "RPD1", "CogsOcrCo2", "2" },
				{ "RPD1", "CogsOcrCo3", "3" },
				{ "RPD1", "CogsOcrCo4", "4" },
				{ "RPD1", "CogsOcrCo5", "5" },
				{ "RPD13", "OcrCode", "1" },
				{ "RPD13", "OcrCode2", "2" },
				{ "RPD13", "OcrCode3", "3" },
				{ "RPD13", "OcrCode4", "4" },
				{ "RPD13", "OcrCode5", "5" },
				{ "IEI3", "OcrCode", "1" },
				{ "IEI3", "OcrCode2", "2" },
				{ "IEI3", "OcrCode3", "3" },
				{ "IEI3", "OcrCode4", "4" },
				{ "IEI3", "OcrCode5", "5" },
				{ "IEI2", "OcrCode", "1" },
				{ "IEI2", "OcrCode2", "2" },
				{ "IEI2", "OcrCode3", "3" },
				{ "IEI2", "OcrCode4", "4" },
				{ "IEI2", "OcrCode5", "5" },
				{ "IEI1", "OcrCode", "1" },
				{ "IEI1", "OcrCode2", "2" },
				{ "IEI1", "OcrCode3", "3" },
				{ "IEI1", "OcrCode4", "4" },
				{ "IEI1", "OcrCode5", "5" },
				{ "IEI1", "CogsOcrCod", "1" },
				{ "IEI1", "CogsOcrCo2", "2" },
				{ "IEI1", "CogsOcrCo3", "3" },
				{ "IEI1", "CogsOcrCo4", "4" },
				{ "IEI1", "CogsOcrCo5", "5" },
				{ "IEI13", "OcrCode", "1" },
				{ "IEI13", "OcrCode2", "2" },
				{ "IEI13", "OcrCode3", "3" },
				{ "IEI13", "OcrCode4", "4" },
				{ "IEI13", "OcrCode5", "5" },
				{ "RCT4", "OcrCode", "1" },
				{ "RCT4", "OcrCode2", "2" },
				{ "RCT4", "OcrCode3", "3" },
				{ "RCT4", "OcrCode4", "4" },
				{ "RCT4", "OcrCode5", "5" },
				{ "ARC4", "OcrCode", "1" },
				{ "ARC4", "OcrCode2", "2" },
				{ "ARC4", "OcrCode3", "3" },
				{ "ARC4", "OcrCode4", "4" },
				{ "ARC4", "OcrCode5", "5" },
				{ "RCC4", "OcrCode", "1" },
				{ "RCC4", "OcrCode2", "2" },
				{ "RCC4", "OcrCode3", "3" },
				{ "RCC4", "OcrCode4", "4" },
				{ "RCC4", "OcrCode5", "5" },
				{ "ARC2", "OcrCode", "1" },
				{ "ARC2", "OcrCode2", "2" },
				{ "ARC2", "OcrCode3", "3" },
				{ "ARC2", "OcrCode4", "4" },
				{ "ARC2", "OcrCode5", "5" },
				{ "RCT2", "OcrCode", "1" },
				{ "RCT2", "OcrCode2", "2" },
				{ "RCT2", "OcrCode3", "3" },
				{ "RCT2", "OcrCode4", "4" },
				{ "RCT2", "OcrCode5", "5" },
				{ "OBTC", "PrftCntFee", "1" },
				{ "OBTC", "PrftCntFe2", "2" },
				{ "OBTC", "PrftCntFe3", "3" },
				{ "OBTC", "PrftCntFe4", "4" },
				{ "OBTC", "PrftCntFe5", "5" },
				{ "BTC1", "PrftCenter", "1" },
				{ "BTC1", "PrftCent2", "2" },
				{ "BTC1", "PrftCent3", "3" },
				{ "BTC1", "PrftCent4", "4" },
				{ "BTC1", "PrftCent5", "5" },
				{ "ABT1", "PrftCenter", "1" },
				{ "ABT1", "PrftCent2", "2" },
				{ "ABT1", "PrftCent3", "3" },
				{ "ABT1", "PrftCent4", "4" },
				{ "ABT1", "PrftCent5", "5" },
				{ "ABTC", "PrftCntFee", "1" },
				{ "ABTC", "PrftCntFe2", "2" },
				{ "ABTC", "PrftCntFe3", "3" },
				{ "ABTC", "PrftCntFe4", "4" },
				{ "ABTC", "PrftCntFe5", "5" },
				{ "AIN1", "OcrCode", "1" },
				{ "AIN1", "OcrCode2", "2" },
				{ "AIN1", "OcrCode3", "3" },
				{ "AIN1", "OcrCode4", "4" },
				{ "AIN1", "OcrCode5", "5" },
				{ "INC1", "OcrCode", "1" },
				{ "INC1", "OcrCode2", "2" },
				{ "INC1", "OcrCode3", "3" },
				{ "INC1", "OcrCode4", "4" },
				{ "INC1", "OcrCode5", "5" },
				{ "ICD1", "OcrCode", "1" },
				{ "ICD1", "OcrCode2", "2" },
				{ "ICD1", "OcrCode3", "3" },
				{ "ICD1", "OcrCode4", "4" },
				{ "ICD1", "OcrCode5", "5" },
				{ "IOD1", "OcrCode", "1" },
				{ "IOD1", "OcrCode2", "2" },
				{ "IOD1", "OcrCode3", "3" },
				{ "IOD1", "OcrCode4", "4" },
				{ "IOD1", "OcrCode5", "5" },
				{ "AQI1", "OcrCode", "1" },
				{ "AQI1", "OcrCode2", "2" },
				{ "AQI1", "OcrCode3", "3" },
				{ "AQI1", "OcrCode4", "4" },
				{ "AQI1", "OcrCode5", "5" },
				{ "IQI1", "OcrCode", "1" },
				{ "IQI1", "OcrCode2", "2" },
				{ "IQI1", "OcrCode3", "3" },
				{ "IQI1", "OcrCode4", "4" },
				{ "IQI1", "OcrCode5", "5" },
				{ "OILM", "OcrCode", "1" },
				{ "OILM", "OcrCode2", "2" },
				{ "OILM", "OcrCode3", "3" },
				{ "OILM", "OcrCode4", "4" },
				{ "OILM", "OcrCode5", "5" },
				{ "AMR1", "OcrCode", "1" },
				{ "AMR1", "OcrCode2", "2" },
				{ "AMR1", "OcrCode3", "3" },
				{ "AMR1", "OcrCode4", "4" },
				{ "AMR1", "OcrCode5", "5" },
				{ "MRV1", "OcrCode", "1" },
				{ "MRV1", "OcrCode2", "2" },
				{ "MRV1", "OcrCode3", "3" },
				{ "MRV1", "OcrCode4", "4" },
				{ "MRV1", "OcrCode5", "5" },
				{ "AQR1", "OcrCode", "1" },
				{ "AQR1", "OcrCode2", "2" },
				{ "AQR1", "OcrCode3", "3" },
				{ "AQR1", "OcrCode4", "4" },
				{ "AQR1", "OcrCode5", "5" },
				{ "IQR1", "OcrCode", "1" },
				{ "IQR1", "OcrCode2", "2" },
				{ "IQR1", "OcrCode3", "3" },
				{ "IQR1", "OcrCode4", "4" },
				{ "IQR1", "OcrCode5", "5" },
				{ "IPD1", "OcrCode", "1" },
				{ "IPD1", "OcrCode2", "2" },
				{ "IPD1", "OcrCode3", "3" },
				{ "IPD1", "OcrCode4", "4" },
				{ "IPD1", "OcrCode5", "5" },
				{ "WTR3", "OcrCode", "1" },
				{ "WTR3", "OcrCode2", "2" },
				{ "WTR3", "OcrCode3", "3" },
				{ "WTR3", "OcrCode4", "4" },
				{ "WTR3", "OcrCode5", "5" },
				{ "WTR2", "OcrCode", "1" },
				{ "WTR2", "OcrCode2", "2" },
				{ "WTR2", "OcrCode3", "3" },
				{ "WTR2", "OcrCode4", "4" },
				{ "WTR2", "OcrCode5", "5" },
				{ "WTR1", "OcrCode", "1" },
				{ "WTR1", "OcrCode2", "2" },
				{ "WTR1", "OcrCode3", "3" },
				{ "WTR1", "OcrCode4", "4" },
				{ "WTR1", "OcrCode5", "5" },
				{ "WTR1", "CogsOcrCod", "1" },
				{ "WTR1", "CogsOcrCo2", "2" },
				{ "WTR1", "CogsOcrCo3", "3" },
				{ "WTR1", "CogsOcrCo4", "4" },
				{ "WTR1", "CogsOcrCo5", "5" },
				{ "WTR13", "OcrCode", "1" },
				{ "WTR13", "OcrCode2", "2" },
				{ "WTR13", "OcrCode3", "3" },
				{ "WTR13", "OcrCode4", "4" },
				{ "WTR13", "OcrCode5", "5" },
				{ "WTQ3", "OcrCode", "1" },
				{ "WTQ3", "OcrCode2", "2" },
				{ "WTQ3", "OcrCode3", "3" },
				{ "WTQ3", "OcrCode4", "4" },
				{ "WTQ3", "OcrCode5", "5" },
				{ "WTQ2", "OcrCode", "1" },
				{ "WTQ2", "OcrCode2", "2" },
				{ "WTQ2", "OcrCode3", "3" },
				{ "WTQ2", "OcrCode4", "4" },
				{ "WTQ2", "OcrCode5", "5" },
				{ "WTQ1", "OcrCode", "1" },
				{ "WTQ1", "OcrCode2", "2" },
				{ "WTQ1", "OcrCode3", "3" },
				{ "WTQ1", "OcrCode4", "4" },
				{ "WTQ1", "OcrCode5", "5" },
				{ "WTQ1", "CogsOcrCod", "1" },
				{ "WTQ1", "CogsOcrCo2", "2" },
				{ "WTQ1", "CogsOcrCo3", "3" },
				{ "WTQ1", "CogsOcrCo4", "4" },
				{ "WTQ1", "CogsOcrCo5", "5" },
				{ "WTQ13", "OcrCode", "1" },
				{ "WTQ13", "OcrCode2", "2" },
				{ "WTQ13", "OcrCode3", "3" },
				{ "WTQ13", "OcrCode4", "4" },
				{ "WTQ13", "OcrCode5", "5" },
				{ "UILM", "OcrCode", "1" },
				{ "UILM", "OcrCode2", "2" },
				{ "UILM", "OcrCode3", "3" },
				{ "UILM", "OcrCode4", "4" },
				{ "UILM", "OcrCode5", "5" },
				{ "AJD1", "ProfitCode", "1" },
				{ "AJD1", "OcrCode2", "2" },
				{ "AJD1", "OcrCode3", "3" },
				{ "AJD1", "OcrCode4", "4" },
				{ "AJD1", "OcrCode5", "5" },
				{ "JDT1", "ProfitCode", "1" },
				{ "JDT1", "OcrCode2", "2" },
				{ "JDT1", "OcrCode3", "3" },
				{ "JDT1", "OcrCode4", "4" },
				{ "JDT1", "OcrCode5", "5" },
				{ "BTF1", "ProfitCode", "1" },
				{ "BTF1", "OcrCode2", "2" },
				{ "BTF1", "OcrCode3", "3" },
				{ "BTF1", "OcrCode4", "4" },
				{ "BTF1", "OcrCode5", "5" },
				{ "IPF1", "OcrCode", "1" },
				{ "IPF1", "OcrCode2", "2" },
				{ "IPF1", "OcrCode3", "3" },
				{ "IPF1", "OcrCode4", "4" },
				{ "IPF1", "OcrCode5", "5" },
				{ "MDP1", "OcrCode", "1" },
				{ "MDP1", "OcrCode2", "2" },
				{ "MDP1", "OcrCode3", "3" },
				{ "MDP1", "OcrCode4", "4" },
				{ "MDP1", "OcrCode5", "5" },
				{ "OEI3", "OcrCode", "1" },
				{ "OEI3", "OcrCode2", "2" },
				{ "OEI3", "OcrCode3", "3" },
				{ "OEI3", "OcrCode4", "4" },
				{ "OEI3", "OcrCode5", "5" },
				{ "OEI2", "OcrCode", "1" },
				{ "OEI2", "OcrCode2", "2" },
				{ "OEI2", "OcrCode3", "3" },
				{ "OEI2", "OcrCode4", "4" },
				{ "OEI2", "OcrCode5", "5" },
				{ "OEI1", "OcrCode", "1" },
				{ "OEI1", "OcrCode2", "2" },
				{ "OEI1", "OcrCode3", "3" },
				{ "OEI1", "OcrCode4", "4" },
				{ "OEI1", "OcrCode5", "5" },
				{ "OEI1", "CogsOcrCod", "1" },
				{ "OEI1", "CogsOcrCo2", "2" },
				{ "OEI1", "CogsOcrCo3", "3" },
				{ "OEI1", "CogsOcrCo4", "4" },
				{ "OEI1", "CogsOcrCo5", "5" },
				{ "OEI13", "OcrCode", "1" },
				{ "OEI13", "OcrCode2", "2" },
				{ "OEI13", "OcrCode3", "3" },
				{ "OEI13", "OcrCode4", "4" },
				{ "OEI13", "OcrCode5", "5" },
				{ "VPM4", "OcrCode", "1" },
				{ "VPM4", "OcrCode2", "2" },
				{ "VPM4", "OcrCode3", "3" },
				{ "VPM4", "OcrCode4", "4" },
				{ "VPM4", "OcrCode5", "5" },
				{ "VPM2", "OcrCode", "1" },
				{ "VPM2", "OcrCode2", "2" },
				{ "VPM2", "OcrCode3", "3" },
				{ "VPM2", "OcrCode4", "4" },
				{ "VPM2", "OcrCode5", "5" },
				{ "PDF4", "OcrCode", "1" },
				{ "PDF4", "OcrCode2", "2" },
				{ "PDF4", "OcrCode3", "3" },
				{ "PDF4", "OcrCode4", "4" },
				{ "PDF4", "OcrCode5", "5" },
				{ "PDF2", "OcrCode", "1" },
				{ "PDF2", "OcrCode2", "2" },
				{ "PDF2", "OcrCode3", "3" },
				{ "PDF2", "OcrCode4", "4" },
				{ "PDF2", "OcrCode5", "5" },
				{ "PWZ3", "OcrCode", "1" },
				{ "PWZ3", "OcrCode2", "2" },
				{ "PWZ3", "OcrCode3", "3" },
				{ "PWZ3", "OcrCode4", "4" },
				{ "PWZ3", "OcrCode5", "5" },
				{ "ODPT", "OcrCode", "1" },
				{ "ODPT", "OcrCode2", "2" },
				{ "ODPT", "OcrCode3", "3" },
				{ "ODPT", "OcrCode4", "4" },
				{ "ODPT", "OcrCode5", "5" },
				{ "TRT1", "OcrCode", "1" },
				{ "TRT1", "OcrCode2", "2" },
				{ "TRT1", "OcrCode3", "3" },
				{ "TRT1", "OcrCode4", "4" },
				{ "TRT1", "OcrCode5", "5" },
				{ "OITT", "OcrCode", "1" },
				{ "OITT", "OcrCode2", "2" },
				{ "OITT", "OcrCode3", "3" },
				{ "OITT", "OcrCode4", "4" },
				{ "OITT", "OcrCode5", "5" },
				{ "AITT", "OcrCode", "1" },
				{ "AITT", "OcrCode2", "2" },
				{ "AITT", "OcrCode3", "3" },
				{ "AITT", "OcrCode4", "4" },
				{ "AITT", "OcrCode5", "5" },
				{ "OWOR", "OcrCode", "1" },
				{ "OWOR", "OcrCode2", "2" },
				{ "OWOR", "OcrCode3", "3" },
				{ "OWOR", "OcrCode4", "4" },
				{ "OWOR", "OcrCode5", "5" },
				{ "UWOR", "OcrCode", "1" },
				{ "UWOR", "OcrCode2", "2" },
				{ "UWOR", "OcrCode3", "3" },
				{ "UWOR", "OcrCode4", "4" },
				{ "UWOR", "OcrCode5", "5" },
				{ "AWOR", "OcrCode", "1" },
				{ "AWOR", "OcrCode2", "2" },
				{ "AWOR", "OcrCode3", "3" },
				{ "AWOR", "OcrCode4", "4" },
				{ "AWOR", "OcrCode5", "5" },
				{ "WOR1", "OcrCode", "1" },
				{ "WOR1", "OcrCode2", "2" },
				{ "WOR1", "OcrCode3", "3" },
				{ "WOR1", "OcrCode4", "4" },
				{ "WOR1", "OcrCode5", "5" },
				{ "UWOR1", "OcrCode", "1" },
				{ "UWOR1", "OcrCode2", "2" },
				{ "UWOR1", "OcrCode3", "3" },
				{ "UWOR1", "OcrCode4", "4" },
				{ "UWOR1", "OcrCode5", "5" },
				{ "AWO1", "OcrCode", "1" },
				{ "AWO1", "OcrCode2", "2" },
				{ "AWO1", "OcrCode3", "3" },
				{ "AWO1", "OcrCode4", "4" },
				{ "AWO1", "OcrCode5", "5" },
				{ "POR3", "OcrCode", "1" },
				{ "POR3", "OcrCode2", "2" },
				{ "POR3", "OcrCode3", "3" },
				{ "POR3", "OcrCode4", "4" },
				{ "POR3", "OcrCode5", "5" },
				{ "POR2", "OcrCode", "1" },
				{ "POR2", "OcrCode2", "2" },
				{ "POR2", "OcrCode3", "3" },
				{ "POR2", "OcrCode4", "4" },
				{ "POR2", "OcrCode5", "5" },
				{ "POR1", "OcrCode", "1" },
				{ "POR1", "OcrCode2", "2" },
				{ "POR1", "OcrCode3", "3" },
				{ "POR1", "OcrCode4", "4" },
				{ "POR1", "OcrCode5", "5" },
				{ "POR1", "CogsOcrCod", "1" },
				{ "POR1", "CogsOcrCo2", "2" },
				{ "POR1", "CogsOcrCo3", "3" },
				{ "POR1", "CogsOcrCo4", "4" },
				{ "POR1", "CogsOcrCo5", "5" },
				{ "POR13", "OcrCode", "1" },
				{ "WTQ13", "OcrCode2", "2" },
				{ "WTQ13", "OcrCode3", "3" },
				{ "WTQ13", "OcrCode4", "4" },
				{ "WTQ13", "OcrCode5", "5" },
				{ "PQT3", "OcrCode", "1" },
				{ "PQT3", "OcrCode2", "2" },
				{ "PQT3", "OcrCode3", "3" },
				{ "PQT3", "OcrCode4", "4" },
				{ "PQT3", "OcrCode5", "5" },
				{ "PQT2", "OcrCode", "1" },
				{ "PQT2", "OcrCode2", "2" },
				{ "PQT2", "OcrCode3", "3" },
				{ "PQT2", "OcrCode4", "4" },
				{ "PQT2", "OcrCode5", "5" },
				{ "PQT1", "OcrCode", "1" },
				{ "PQT1", "OcrCode2", "2" },
				{ "PQT1", "OcrCode3", "3" },
				{ "PQT1", "OcrCode4", "4" },
				{ "PQT1", "OcrCode5", "5" },
				{ "PQT1", "CogsOcrCod", "1" },
				{ "PQT1", "CogsOcrCo2", "2" },
				{ "PQT1", "CogsOcrCo3", "3" },
				{ "PQT1", "CogsOcrCo4", "4" },
				{ "PQT1", "CogsOcrCo5", "5" },
				{ "PQT13", "OcrCode", "1" },
				{ "PQT13", "OcrCode2", "2" },
				{ "PQT13", "OcrCode3", "3" },
				{ "PQT13", "OcrCode4", "4" },
				{ "PQT13", "OcrCode5", "5" },
				{ "PRQ3", "OcrCode", "1" },
				{ "PRQ3", "OcrCode2", "2" },
				{ "PRQ3", "OcrCode3", "3" },
				{ "PRQ3", "OcrCode4", "4" },
				{ "PRQ3", "OcrCode5", "5" },
				{ "PRQ2", "OcrCode", "1" },
				{ "PRQ2", "OcrCode2", "2" },
				{ "PRQ2", "OcrCode3", "3" },
				{ "PRQ2", "OcrCode4", "4" },
				{ "PRQ2", "OcrCode5", "5" },
				{ "PRQ1", "OcrCode", "1" },
				{ "PRQ1", "OcrCode2", "2" },
				{ "PRQ1", "OcrCode3", "3" },
				{ "PRQ1", "OcrCode4", "4" },
				{ "PRQ1", "OcrCode5", "5" },
				{ "PRQ1", "CogsOcrCod", "1" },
				{ "PRQ1", "CogsOcrCo2", "2" },
				{ "PRQ1", "CogsOcrCo3", "3" },
				{ "PRQ1", "CogsOcrCo4", "4" },
				{ "PRQ1", "CogsOcrCo5", "5" },
				{ "PRQ13", "OcrCode", "1" },
				{ "PRQ13", "OcrCode2", "2" },
				{ "PRQ13", "OcrCode3", "3" },
				{ "PRQ13", "OcrCode4", "4" },
				{ "PRQ13", "OcrCode5", "5" },
				{ "RCR1", "OcrCode1", "1" },
				{ "RCR1", "OcrCode2", "2" },
				{ "RCR1", "OcrCode3", "3" },
				{ "RCR1", "OcrCode4", "4" },
				{ "RCR1", "OcrCode5", "5" },
				{ "OFLT", "OcrFrom2", "1" },
				{ "OFLT", "OcrFrom3", "3" },
				{ "OFLT", "OcrFrom4", "4" },
				{ "OFLT", "OcrFrom5", "5" },
				{ "OFLT", "OcrTo2", "2" },
				{ "OFLT", "OcrTo3", "3" },
				{ "OFLT", "OcrTo4", "4" },
				{ "OFLT", "OcrTo5", "5" },
				{ "RTI1", "OcrCode", "1" },
				{ "RTI1", "OcrCode2", "2" },
				{ "RTI1", "OcrCode3", "3" },
				{ "RTI1", "OcrCode4", "4" },
				{ "RTI1", "OcrCode5", "5" },
				{ "RDN3", "OcrCode", "1" },
				{ "RDN3", "OcrCode2", "2" },
				{ "RDN3", "OcrCode3", "3" },
				{ "RDN3", "OcrCode4", "4" },
				{ "RDN3", "OcrCode5", "5" },
				{ "RDN2", "OcrCode", "1" },
				{ "RDN2", "OcrCode2", "2" },
				{ "RDN2", "OcrCode3", "3" },
				{ "RDN2", "OcrCode4", "4" },
				{ "RDN2", "OcrCode5", "5" },
				{ "RDN1", "OcrCode", "1" },
				{ "RDN1", "OcrCode2", "2" },
				{ "RDN1", "OcrCode3", "3" },
				{ "RDN1", "OcrCode4", "4" },
				{ "RDN1", "OcrCode5", "5" },
				{ "RDN1", "CogsOcrCod", "1" },
				{ "RDN1", "CogsOcrCo2", "2" },
				{ "RDN1", "CogsOcrCo3", "3" },
				{ "RDN1", "CogsOcrCo4", "4" },
				{ "RDN1", "CogsOcrCo5", "5" },
				{ "RDN13", "OcrCode", "1" },
				{ "RDN13", "OcrCode2", "2" },
				{ "RDN13", "OcrCode3", "3" },
				{ "RDN13", "OcrCode4", "4" },
				{ "RDN13", "OcrCode5", "5" },
				{ "RDR3", "OcrCode", "1" },
				{ "RDR3", "OcrCode2", "2" },
				{ "RDR3", "OcrCode3", "3" },
				{ "RDR3", "OcrCode4", "4" },
				{ "RDR3", "OcrCode5", "5" },
				{ "RDR2", "OcrCode", "1" },
				{ "RDR2", "OcrCode2", "2" },
				{ "RDR2", "OcrCode3", "3" },
				{ "RDR2", "OcrCode4", "4" },
				{ "RDR2", "OcrCode5", "5" },
				{ "RDR1", "OcrCode", "1" },
				{ "RDR1", "OcrCode2", "2" },
				{ "RDR1", "OcrCode3", "3" },
				{ "RDR1", "OcrCode4", "4" },
				{ "RDR1", "OcrCode5", "5" },
				{ "RDR1", "CogsOcrCod", "1" },
				{ "RDR1", "CogsOcrCo2", "2" },
				{ "RDR1", "CogsOcrCo3", "3" },
				{ "RDR1", "CogsOcrCo4", "4" },
				{ "RDR1", "CogsOcrCo5", "5" },
				{ "RDR13", "OcrCode", "1" },
				{ "RDR13", "OcrCode2", "2" },
				{ "RDR13", "OcrCode3", "3" },
				{ "RDR13", "OcrCode4", "4" },
				{ "RDR13", "OcrCode5", "5" },
				{ "QUT3", "OcrCode", "1" },
				{ "QUT3", "OcrCode2", "2" },
				{ "QUT3", "OcrCode3", "3" },
				{ "QUT3", "OcrCode4", "4" },
				{ "QUT3", "OcrCode5", "5" },
				{ "QUT2", "OcrCode", "1" },
				{ "QUT2", "OcrCode2", "2" },
				{ "QUT2", "OcrCode3", "3" },
				{ "QUT2", "OcrCode4", "4" },
				{ "QUT2", "OcrCode5", "5" },
				{ "QUT1", "OcrCode", "1" },
				{ "QUT1", "OcrCode2", "2" },
				{ "QUT1", "OcrCode3", "3" },
				{ "QUT1", "OcrCode4", "4" },
				{ "QUT1", "OcrCode5", "5" },
				{ "QUT1", "CogsOcrCod", "1" },
				{ "QUT1", "CogsOcrCo2", "2" },
				{ "QUT1", "CogsOcrCo3", "3" },
				{ "QUT1", "CogsOcrCo4", "4" },
				{ "QUT1", "CogsOcrCo5", "5" },
				{ "QUT13", "OcrCode", "1" },
				{ "QUT13", "OcrCode2", "2" },
				{ "QUT13", "OcrCode3", "3" },
				{ "QUT13", "OcrCode4", "4" },
				{ "QUT13", "OcrCode5", "5" },
				{ "SCM2", "PrfCntVal", "1" },
				{ "SCR2", "PrfCntVal", "1" },
				{ "SLM1", "PrfCntVal", "1" },
				{ "SLM1", "PrfCntOrig", "1" },
				{ "SLR1", "PrfCntVal", "1" },
				{ "SLR1", "PrfCntOrig", "1" },
				{ "FTR1", "OcrCode", "1" },
				{ "FTR1", "OcrCode2", "2" },
				{ "FTR1", "OcrCode3", "3" },
				{ "FTR1", "OcrCode4", "4" },
				{ "FTR1", "OcrCode5", "5" },
				{ "OINM", "OcrCode", "1" },
				{ "OINM", "OcrCode2", "2" },
				{ "OINM", "OcrCode3", "3" },
				{ "OINM", "OcrCode4", "4" },
				{ "OINM", "OcrCode5", "5" }
			};
			StringBuilder stringBuilder = new StringBuilder();
			List<Tuple<string, string>> list = new List<Tuple<string, string>>();
			int length = array.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				if (Function.ParseInt(array[i, 2]) == dimension)
				{
					list.Add(new Tuple<string, string>(array[i, 0], array[i, 1]));
				}
			}
			stringBuilder.Append(DeclareVariables(key));
			for (int j = 0; j < list.Count; j++)
			{
				stringBuilder.Append(GetSQLCheckQuery(list[j].Item1 ?? "", list[j].Item2 ?? ""));
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR:" + ex.Message;
		}
	}

	public DataTable GetStructRow()
	{
		try
		{
			return ExecuteDataTable("SELECT TOP 0 * FROM OCR1;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	protected virtual string SearchQuery(string ocrCode, string ocrName, int dimesion)
	{
		string text = "select a.*,(Select top 1 ValidFrom From OCR1 t Where t.OcrCode = a.OcrCode) ValidFrom\r\n\t                        ,(Select top 1 ValidTo From OCR1 t Where t.OcrCode = a.OcrCode) ValidTo\r\n                            from OOCR a WHERE 1=1 ";
		if (!string.IsNullOrEmpty(ocrCode))
		{
			text += "AND  a.OcrCode like '%' + @OcrCode + '%'";
		}
		if (!string.IsNullOrEmpty(ocrName))
		{
			text += "AND  a.OcrName like '%' + @OcrName + '%'";
		}
		if (dimesion > 0)
		{
			text += "AND  a.DimCode = @DimCode ";
		}
		return text;
	}

	protected virtual string GetSQLCheckQuery(string tableName, string column)
	{
		return "if exists(select 1 from " + tableName + " where " + column + "=@key)\r\n                    begin\r\n                        select 'This distribute rule is used in table " + tableName + "'\r\n                        return\r\n                    end" + Environment.NewLine;
	}

	protected virtual string DeclareVariables(string key)
	{
		return $"declare @countDist int=0;";
	}
}
