using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class EmpRelativesRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IEmpRelatives
{
	private string Temp_Doc = "#APZ_EmpRela_DOC";

	private string Temp_Atc = "#APZ_EmpRela_ATC";

	public DataSet GetDetail(int relaId, int empId)
	{
		try
		{
			string query = "apz_EmpRelatives_GetDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@relaId", relaId));
			list.Add(new SqlParameter("@empId", empId));
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

	public string SaveDocDataSet(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transactionType = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transactionType = Enums.TransactionType.A;
				break;
			case "U":
				transactionType = Enums.TransactionType.U;
				break;
			case "D":
				transactionType = Enums.TransactionType.D;
				break;
			default:
				return "EmpRelative Master data's Repository: Wrong action. Should be: 'A', 'U' OR 'D'";
			}
			return Process(ApzObjectTypes.oEmpRelatives, -1, transactionType, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int relaId, int empId)
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("relaId", typeof(int));
			dataTable.Columns.Add("empId", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["relaId"] = relaId;
			dataRow["empId"] = empId;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataSet.Tables.Add(dataTable.Copy());
			return Process(ApzObjectTypes.oEmpRelatives, -1, Enums.TransactionType.D, dataSet).Message;
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
			ProcessDelete(ds, ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables.Count < 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				string value = ((transtype == Enums.TransactionType.A) ? "A" : "U");
				CreateTempTable(ds.Tables["dtDoc"], Temp_Doc);
				BulkCopy(ds.Tables["dtDoc"], Temp_Doc);
				CreateTempTable(ds.Tables["AttFile"], Temp_Atc);
				BulkCopy(ds.Tables["AttFile"], Temp_Atc);
				SqlParameter sqlParameter = new SqlParameter("@Type", value);
				DataTable dataTable = ExecuteDataTable("apz_process_EmpRelatives", CommandType.StoredProcedure, sqlParameter);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
				else
				{
					ret.Message = "";
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(ds.Tables[0].Rows[0]["EmpID"]);
				objNum = Function.ToString(ds.Tables[0].Rows[0]["EmpID"]);
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

	private void ProcessDelete(DataSet ds, ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@relaId", Function.ToString(ds.Tables[0].Rows[0]["relaId"])));
			list.Add(new SqlParameter("@empId", Function.ToString(ds.Tables[0].Rows[0]["empId"])));
			int num = ExecuteNonQuery("apz_process_EmpRelatives_Remove", CommandType.StoredProcedure, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public bool CheckDoc(int relaId, int empId)
	{
		try
		{
			string query = "SELECT empID\r\n                            FROM APZ_OHER\r\n                            WHERE relaID = @relaId AND empID = @empId";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@relaId", relaId));
			list.Add(new SqlParameter("@empId", empId));
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

	public int GetNextId(int empId)
	{
		try
		{
			string query = GetNextIdQuery() + empId + ";";
			return Function.ParseInt(ExecuteScalar(query, CommandType.Text).ToString()) + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return 0;
		}
	}

	protected virtual string GetNextIdQuery()
	{
		return "select max(relaID) from apz_oher where empID = ";
	}

	public DataTable ExistFileInDoc(int relaId, int empId, string fileName, string fileExt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = ExistFileInDocQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@relaID", relaId));
			list.Add(new SqlParameter("@empID", empId));
			list.Add(new SqlParameter("@fileExt", fileExt));
			list.Add(new SqlParameter("@fileName", fileName));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistFileInDocQuery()
	{
		return "select FileName from atc1 where AbsEntry=(select AtcEntry from apz_oher where relaID = @relaID and empID=@empID) \r\n\t                and FileExt=@fileExt and FileName=@fileName and AbsEntry is not null";
	}

	public DataTable GetAllRecords(int empId)
	{
		try
		{
			string allRecordsQuery = GetAllRecordsQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@empId", empId));
			DataTable dataTable = ExecuteDataTable(allRecordsQuery, CommandType.Text, list.ToArray());
			if (dataTable != null)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDetail  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetAllRecordsQuery()
	{
		return "SELECT * FROM APZ_OHER WHERE empID = @empId";
	}

	public string GetRecord(int relaId, int empId, int type)
	{
		DataTable dataTable = new DataTable();
		int num = 0;
		try
		{
			string query = "apz_EmpRelatives_navigation";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Current", relaId));
			list.Add(new SqlParameter("@EmpID", empId));
			list.Add(new SqlParameter("@Type", type));
			dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num = dataTable.Rows[0].Field<int>("relaID");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord ", ex.Message);
		}
		return num.ToString();
	}

	public DataTable Search(DataTable dtDoc)
	{
		try
		{
			string text = "select relaID from apz_oher where 1=1 ";
			List<SqlParameter> list = new List<SqlParameter>();
			for (int i = 0; i < dtDoc.Columns.Count; i++)
			{
				if (dtDoc.Rows[0][dtDoc.Columns[i].ColumnName] == null)
				{
					continue;
				}
				switch (dtDoc.Columns[i].DataType.Name.ToUpper())
				{
				case "DATETIME":
				{
					DateTime? dateTime = Function.ParseDateTime(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (dateTime.HasValue && dateTime > new DateTime(1900, 1, 1))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, dateTime));
						text = text + " and " + dtDoc.Columns[i].ColumnName + "=@" + dtDoc.Columns[i].ColumnName;
					}
					break;
				}
				case "INT64":
				case "INT32":
				case "INT16":
				{
					int num = Function.ParseInt(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (num != 0)
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, num));
						text = text + " and " + dtDoc.Columns[i].ColumnName + "=@" + dtDoc.Columns[i].ColumnName;
					}
					break;
				}
				case "STRING":
				{
					string text2 = Function.ToString(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, "%" + text2 + "%"));
						text = text + " and " + dtDoc.Columns[i].ColumnName + " COLLATE SQL_Latin1_General_Cp1_CI_AI like @" + dtDoc.Columns[i].ColumnName + " COLLATE SQL_Latin1_General_Cp1_CI_AI ";
					}
					break;
				}
				default:
				{
					string value = Function.ToString(dtDoc.Rows[0][dtDoc.Columns[i].ColumnName]);
					if (!string.IsNullOrEmpty(value))
					{
						list.Add(new SqlParameter("@" + dtDoc.Columns[i].ColumnName, value));
						text = text + " and " + dtDoc.Columns[i].ColumnName + "=@" + dtDoc.Columns[i].ColumnName;
					}
					break;
				}
				}
			}
			return ExecuteDataTable(text, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
