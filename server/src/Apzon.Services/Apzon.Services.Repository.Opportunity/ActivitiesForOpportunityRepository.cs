using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Opportunity;

namespace Apzon.Services.Repository.Opportunity;

public class ActivitiesForOpportunityRepository : BaseService<DataSet>, IActivitiesForOpportunity
{
	public ActivitiesForOpportunityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDefaultTable()
	{
		try
		{
			string query = "APZ_ActivityForOpportunity_GetDefaultTable";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDefaultTable  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetMaxActivity_ClgCode()
	{
		try
		{
			string query = "APZ_ActivitiesForOpportunity_GetMax";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxActivity_ClgCode  FAILED ", ex.Message);
			return -1;
		}
	}

	public int GetMinClgCode()
	{
		try
		{
			string query = "select min(ClgCode) from oclg";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxActivity_ClgCode  FAILED ", ex.Message);
			return -1;
		}
	}

	public DataTable GetActivityByClgCode(int clgCode)
	{
		try
		{
			return ExecuteDataTable("select * from oclg where ClgCode = @clgCode", CommandType.Text, new SqlParameter("@clgCode", clgCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetMaxActivity_ClgCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataTable dt, List<AttachmentFile_Impl> lstAttachment)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			if (Function.ParseInt(dt.Rows[0]["ClgCode"]) > 0)
			{
				transtype = Enums.TransactionType.U;
			}
			dt.TableName = "DOC";
			DataTable dataTable = EntityToDataTable(lstAttachment, "ATC1");
			dataTable.TableName = "ATT";
			DataTable[] array = new DataTable[2] { dt, dataTable };
			for (int i = 0; i < array.Length; i++)
			{
				DataSet dataSet = array[i].DataSet;
				if (dataSet != null)
				{
					dataSet.Tables.Remove(array[i]);
					dataSet.AcceptChanges();
				}
			}
			DataSet dataSet2 = new DataSet();
			dataSet2.Tables.AddRange(array);
			HttpResult httpResult = Process(ApzObjectTypes.oContacts, -1, transtype, dataSet2);
			if (httpResult.Message == "")
			{
				return "OK:" + httpResult.Content;
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Doc  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessActivity(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessActivity(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDelete(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessActivity(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			string text = ((transtype == Enums.TransactionType.A) ? "A" : "U");
			string text2 = "#APZ_Acitivty_DOC";
			string tblName = "#APZ_Acitivty_ATC";
			if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				CreateTempTable(ds.Tables[0], text2);
				BulkCopy(ds.Tables[0], text2);
				CreateTempTable(ds.Tables[1], tblName);
				BulkCopy(ds.Tables[1], tblName);
				SqlParameter sqlParameter = new SqlParameter("@Type", text);
				DataTable dataTable = ExecuteDataTable("APZ_ActivityForOpportunity_SaveDoc", CommandType.StoredProcedure, sqlParameter);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text3 = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text3 + " - " + ret.Message;
				}
				else
				{
					ret.Message = "";
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				if (text == "U")
				{
					objId = Function.ToString(ds.Tables[0].Rows[0]["ClgCode"]);
					objNum = Function.ToString(ds.Tables[0].Rows[0]["ClgCode"]);
					return;
				}
				DataTable dataTable2 = ExecuteDataTable("Select ClgCode From " + text2, CommandType.Text);
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					objId = Function.ToString(dataTable2.Rows[0][0]);
					objNum = Function.ToString(dataTable2.Rows[0][0]);
				}
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

	private void ProcessDelete(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@id", Function.ParseInt(objId));
			DataTable dataTable = ExecuteDataTable("APZ_ActivitesForOpportunity_Delete", CommandType.StoredProcedure, sqlParameter);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				ret.Message = "";
			}
			else
			{
				ret.Message = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
			ret.Message = "Update error: " + ex.Message;
		}
	}

	public string Delete(int id)
	{
		return Process(ApzObjectTypes.oContacts, -1, Enums.TransactionType.D, new DataSet(), id.ToString()).Message;
	}

	public DataTable ExistFileInDoc(int ClgCode, string fileName, string fileExt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select FileName from atc1 where AbsEntry=(select AtcEntry from OCLG where ClgCode=@clgCode) \r\n\t                and FileExt=@fileExt and FileName=@fileName and AbsEntry is not null";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@clgCode", ClgCode));
			list.Add(new SqlParameter("@fileExt", fileExt));
			list.Add(new SqlParameter("@fileName", fileName));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSearchTable  ", ex.Message);
		}
		return result;
	}

	private DataTable EntityToDataTable(object entitiese, string tableName)
	{
		try
		{
			if (tableName == "ATC1")
			{
				DataTable bPTableStructure = GetUnitOfWork().BusinessMasterData.GetBPTableStructure(tableName);
				if (entitiese is List<AttachmentFile_Impl> list)
				{
					for (int i = 0; i < list.Count; i++)
					{
						DataRow row = bPTableStructure.NewRow();
						Function.ObjectToRow(list[i], row);
						bPTableStructure.Rows.Add(row);
					}
					bPTableStructure.AcceptChanges();
				}
				return bPTableStructure;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchDoc(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			int num = Function.ParseInt(dt.Rows[0]["ClgCode"]);
			int num2 = Function.ParseInt(dt.Rows[0]["AttendUser"]);
			int num3 = Function.ParseInt(dt.Rows[0]["AttendEmpl"]);
			int num4 = Function.ParseInt(dt.Rows[0]["DocType"]);
			string value = Function.ToString(dt.Rows[0]["CardCode"]);
			DateTime? dateTime = Function.ParseDateTime(dt.Rows[0]["Recontact"]);
			DateTime? dateTime2 = Function.ParseDateTime(dt.Rows[0]["endDate"]);
			string query = "APZ_ActivitiesForOpportunity_SearchDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			if (num > 0)
			{
				list.Add(new SqlParameter("@ClgCode", num));
			}
			if (num4 > 0)
			{
				list.Add(new SqlParameter("@DocType", num4));
			}
			if (num2 > 0)
			{
				list.Add(new SqlParameter("@AttendUser", num2));
			}
			if (num3 > 0)
			{
				list.Add(new SqlParameter("@AttendEmpl", num3));
			}
			if (!string.IsNullOrWhiteSpace(value))
			{
				list.Add(new SqlParameter("@CardCode", Function.ToString(dataRow["CardCode"])));
			}
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				list.Add(new SqlParameter("@StartTime", dateTime.Value));
			}
			if (dateTime2.HasValue)
			{
				_ = dateTime2.Value;
				list.Add(new SqlParameter("@EndTime", dateTime2));
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchActivityOverviewBP(DataTable dt)
	{
		try
		{
			DataRow dataRow = dt.Rows[0];
			string value = ((dt.Rows[0]["CardCode"].ToString() != null) ? dt.Rows[0]["CardCode"].ToString() : string.Empty);
			string value2 = ((dt.Rows[0]["inactive"].ToString() != null) ? dt.Rows[0]["inactive"].ToString() : string.Empty);
			string query = "APZ_ActivitiesOverviewBP_SearchDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrWhiteSpace(value))
			{
				list.Add(new SqlParameter("@CardCode", Function.ToString(dataRow["CardCode"])));
			}
			if (!string.IsNullOrWhiteSpace(value2))
			{
				list.Add(new SqlParameter("@inactive", Function.ToString(dataRow["inactive"])));
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public int CheckExistAttachment(int clgCode)
	{
		try
		{
			return Function.ParseInt(ExecuteDataTable("SELECT AtcEntry FROM OCLG WHERE ClgCode=@clgCode", CommandType.Text, new SqlParameter("@clgCode", clgCode)).Rows[0]["AtcEntry"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckExistAttachment  FAILED ", ex.Message);
			return 0;
		}
	}

	public DataTable GetFirstRecord()
	{
		return GetUnitOfWork().Commons.GetFirstRecord("oclg", "ClgCode");
	}

	public DataTable GetLastRecord()
	{
		return GetUnitOfWork().Commons.GetLastRecord("oclg", "ClgCode");
	}

	public DataTable GetNextRecord(string id)
	{
		return GetUnitOfWork().Commons.GetNextRecord("oclg", "ClgCode", id);
	}

	public DataTable GetPreviousRecord(string id)
	{
		return GetUnitOfWork().Commons.GetPreviousRecord("oclg", "ClgCode", id);
	}

	public int GetRecord(string position, int clgCode)
	{
		try
		{
			string searchQuery = GetSearchQuery(position);
			DataTable dataTable = ExecuteDataTable(searchQuery, CommandType.Text, new SqlParameter("@clgCode", clgCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	protected virtual string GetSearchQuery(string position)
	{
		string result = "";
		switch (position)
		{
		case "F":
			result = "Select top 1 \"ClgCode\" From OCLG order by \"ClgCode\" asc;";
			break;
		case "P":
			result = " With CTE as (\r\n                            select RN = ROW_NUMBER() OVER (ORDER BY ClgCode), ClgCode FROM OCLG)\r\n                             SELECT [Previous Row].ClgCode FROM CTE [Current Row]\r\n                             LEFT JOIN CTE [Previous Row] ON \r\n                             [Previous Row].RN = [Current Row].RN - 1\r\n                            WHERE [Current Row].clgCode = @clgCode";
			break;
		case "N":
			result = " With CTE as (\r\n                            select RN = ROW_NUMBER() OVER (ORDER BY ClgCode), ClgCode FROM OCLG)\r\n                            SELECT [Next Row].ClgCode FROM CTE [Current Row]\r\n                            LEFT JOIN CTE [Next Row] ON \r\n                            [Next Row].RN = [Current Row].RN + 1\r\n                            WHERE [Current Row].ClgCode = @clgCode";
			break;
		case "L":
			result = "Select top 1 \"ClgCode\" From OCLG order by \"ClgCode\" desc;";
			break;
		}
		return result;
	}

	public bool CheckDoc(string clgCode)
	{
		try
		{
			string query = "SELECT 1\r\n                            FROM OCLG \r\n                            WHERE \"ClgCode\" = @clgCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@clgCode", clgCode));
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

	public DataTable LoadOneDoc(int clgCode)
	{
		try
		{
			return ExecuteDataTable("APZ_ActivitiesForOpportunity_LoadOneDoc", CommandType.StoredProcedure, new SqlParameter("@clgCode", clgCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetCountries()
	{
		try
		{
			return ExecuteDataTable("Select \"Code\",\"Name\" From OCRY;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetStates(string country)
	{
		try
		{
			return ExecuteDataTable("Select \"Code\",\"Name\" From OCST Where \"Country\" = @Country;", CommandType.Text, new SqlParameter("@Country", country));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string GetLinkedObject(int clgCode)
	{
		try
		{
			string query = "Select top 1 'S' \"Name\" From SCL5 Where \"ClgID\" = @clgCode  union all Select top 1 'B' \"Name\" From OAT3 Where \"ActivityID\" = @clgCode union all Select top 1 'O' \"Name\" From OCLG Where \"ClgCode\" = @clgCode and \"OprId\" > 0;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@clgCode", clgCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR:" + ex.Message;
		}
	}
}
