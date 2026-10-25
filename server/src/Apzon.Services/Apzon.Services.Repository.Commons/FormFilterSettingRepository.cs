using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class FormFilterSettingRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IFormFilterSetting
{
	public string tempTable = "#APZ_FILTERSETTING";

	public string tempTableLine = "#APZ_FILTERSETTINGLINE";

	public string tempTableLine2 = "#APZ_FILTERSETTINGLINE2";

	public string filter = "";

	public DataTable GetDataFilterField(ApzObjectTypes objectType)
	{
		try
		{
			return ExecuteDataTable("SELECT \"DocEntry\", \"FilterCode\", \"FilterName\", \"SourceType\" from APZ_FILTERSETTING where \"ObjType\" = @objType and \"Active\" = 'Y';", CommandType.Text, new SqlParameter("@objType", (int)objectType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataFilterSetting(ApzObjectTypes objectType)
	{
		try
		{
			return ExecuteDataTable("SELECT * from APZ_FILTERSETTING where \"ObjType\" = @objType;", CommandType.Text, new SqlParameter("@objType", (int)objectType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult SaveFilterSetting(Tuple<DataSet, ApzObjectTypes> tuple)
	{
		try
		{
			return Process(tuple.Item2, -1, Enums.TransactionType.U, tuple.Item1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveData  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveFilterSettingAddNew(Tuple<DataTable, ApzObjectTypes, DataRow> tuple)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(tuple.Item1);
			DataTable table = tuple.Item3.Table;
			table.Rows.Add(tuple.Item3);
			dataSet.Tables.Add(table);
			filter = "FilterSetting";
			return Process(tuple.Item2, -1, Enums.TransactionType.A, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveData  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult ChangeSourceType(DataTable dt)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			filter = "FilterSettingChangeSourceType";
			return Process(ApzObjectTypes.oNone, -1, Enums.TransactionType.D, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Change Source Type  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult DeleteFilterSetting(int docEntry)
	{
		try
		{
			string query = "delete from APZ_FILTERSETTINGLINE where \"DocEntry\" = @docEntry;\r\n                            delete from APZ_FILTERSETTINGLINE2 where \"DocEntry\" = @docEntry;\r\n                            delete from APZ_FILTERSETTING where \"DocEntry\" = @docEntry;";
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@docEntry", docEntry));
				dbTransaction.Commit();
				return new HttpResult();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetFilterStructure()
	{
		try
		{
			string query = "select top 0 * from APZ_FILTERSETTING;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Structure  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataFilterValue(int docEntry)
	{
		try
		{
			return ExecuteDataTable("SELECT Code, Name from APZ_FILTERSETTINGLINE where DocEntry = @docEntry and [Active] = 'Y'", CommandType.Text, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataFilterSettingLine(int docEntry)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_FILTERSETTINGLINE where \"DocEntry\"=@docEntry;", CommandType.Text, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult SaveFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(tuple.Item1);
			filter = "FilterSettingLine";
			return Process(tuple.Item2, -1, Enums.TransactionType.U, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveData  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult SaveFilterSettingLineAddNew(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(tuple.Item1);
			filter = "FilterSettingLine";
			return Process(tuple.Item2, -1, Enums.TransactionType.A, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveData  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult DeleteFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(tuple.Item1);
			filter = "FilterSettingLine";
			return Process(tuple.Item2, -1, Enums.TransactionType.D, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteData  FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetDataFilterSettingTable(int docEntry)
	{
		try
		{
			return ExecuteDataTable("select * from  APZ_FILTERSETTINGLINE2 where \"DocEntry\"=@docEntry;", CommandType.Text, new SqlParameter("@docEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataTableByTableName(string tableName)
	{
		try
		{
			return ExecuteDataTable(GetListColumnQuery(), CommandType.Text, new SqlParameter("@tableName", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetListColumnQuery()
	{
		return "SELECT COLUMN_NAME\r\n                                    FROM INFORMATION_SCHEMA.COLUMNS\r\n                                    WHERE TABLE_NAME = @tableName ORDER BY ORDINAL_POSITION";
	}

	public HttpResult SaveFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(tuple.Item1);
			filter = "FilterSettingTable";
			return Process(tuple.Item2, -1, Enums.TransactionType.U, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult();
		}
	}

	public HttpResult DeleteFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(tuple.Item1);
			filter = "FilterSettingTable";
			return Process(tuple.Item2, -1, Enums.TransactionType.D, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult();
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		try
		{
			CreateTempTable(ds.Tables[0], "#APZ_FILTERSETTING");
			BulkCopy(ds.Tables[0], "#APZ_FILTERSETTING");
			CreateTempTable(ds.Tables[1], "#APZ_FILTERSETTINGLINE");
			BulkCopy(ds.Tables[1], "#APZ_FILTERSETTINGLINE");
			CreateTempTable(ds.Tables[2], "#APZ_FILTERSETTINGLINE2");
			BulkCopy(ds.Tables[2], "#APZ_FILTERSETTINGLINE2");
			string query = "APZ_FormFilterSetting_Save";
			ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@obj", (int)objtype));
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult AddNew(ApzObjectTypes objType, int bplId, DataSet ds, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		try
		{
			objId = "";
			objNum = "";
			if (filter == "FilterSetting")
			{
				DataTable dt = ds.Tables[0];
				DataTable dataTable = ds.Tables[1];
				CreateTempTable(dt, tempTable);
				BulkCopy(dt, tempTable);
				string query = "APZ_FormFilterSetting_AddNew";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@obj", (int)objType));
				list.Add(new SqlParameter("@filterCode", Function.ToString(dataTable.Rows[0]["FilterCode"])));
				list.Add(new SqlParameter("@filterName", Function.ToString(dataTable.Rows[0]["FilterName"])));
				list.Add(new SqlParameter("@active", Function.ToString(dataTable.Rows[0]["Active"])));
				list.Add(new SqlParameter("@remarks", Function.ToString(dataTable.Rows[0]["Remarks"])));
				list.Add(new SqlParameter("@sourceType", Function.ToString(dataTable.Rows[0]["SourceType"])));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			if (filter == "FilterSettingLine")
			{
				DataTable dataTable2 = ds.Tables[0];
				CreateTempTable(dataTable2, tempTableLine);
				BulkCopy(dataTable2, tempTableLine);
				string query2 = "APZ_FilterSettingLine_AddNew";
				List<SqlParameter> list2 = new List<SqlParameter>();
				list2.Add(new SqlParameter("@docEntry", Function.ParseInt(dataTable2.Rows[0]["DocEntry"])));
				if (ExecuteNonQuery(query2, CommandType.StoredProcedure, list2.ToArray()) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Update(ApzObjectTypes objtype, int bplId, DataSet ds, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		try
		{
			objId = "";
			objNum = "";
			DataTable dataTable = ds.Tables[0];
			if (filter == "FilterSetting")
			{
				CreateTempTable(dataTable, tempTable);
				BulkCopy(dataTable, tempTable);
				string query = "delete APZ_FILTERSETTING where ObjType= @obj  insert into APZ_FILTERSETTING(DocEntry,ObjType,FilterCode,FilterName,Active,Remarks,SourceType) select DocEntry,ObjType,FilterCode,FilterName,Active,Remarks,SourceType from #APZ_FILTERSETTING";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@obj", (int)objtype)) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			if (filter == "FilterSettingLine")
			{
				CreateTempTable(dataTable, tempTableLine);
				BulkCopy(dataTable, tempTableLine);
				string query2 = " DELETE FROM APZ_FILTERSETTINGLINE WHERE DocEntry =@docEntry INSERT INTO APZ_FILTERSETTINGLINE(DocEntry,LineNum,Code,Name,Active,Remarks) select DocEntry,LineNum,Code,Name,Active,Remarks from #APZ_FILTERSETTINGLINE";
				if (ExecuteNonQuery(query2, CommandType.Text, new SqlParameter("@docEntry", Function.ParseInt(dataTable.Rows[0][0]))) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			if (filter == "FilterSettingTable")
			{
				CreateTempTable(dataTable, tempTableLine2);
				BulkCopy(dataTable, tempTableLine2);
				string query3 = "APZ_FormFilterSetting_SaveFilterSettingTable";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@DocEntry", Function.ParseInt(dataTable.Rows[0]["DocEntry"])));
				list.Add(new SqlParameter("@tableName", Function.ToString(dataTable.Rows[0]["TableName"])));
				list.Add(new SqlParameter("@Code", Function.ToString(dataTable.Rows[0]["Code"])));
				list.Add(new SqlParameter("@Name", Function.ToString(dataTable.Rows[0]["Name"])));
				list.Add(new SqlParameter("@RowCount", Function.ParseInt(0)));
				DataTable dataTable2 = ExecuteDataTable(query3, CommandType.StoredProcedure, list.ToArray());
				if (Function.ToString(dataTable2.Rows[0][1]) == "Success")
				{
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable2.Rows[0][1]));
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Delete(ApzObjectTypes objtype, int bplId, DataSet ds, ref string objId, ref string objNum, ref Enums.TransactionType transtype)
	{
		try
		{
			objId = "";
			objNum = "";
			DataTable dataTable = ds.Tables[0];
			if (filter == "FilterSetting")
			{
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@DocEntry", Function.ToString(dataTable.Rows[0][0])));
				if (ExecuteNonQuery("DELETE APZ_FILTERSETTING where DocEntry = @DocEntry ; DELETE APZ_FILTERSETTINGLINE where DocEntry = @DocEntry ;DELETE APZ_FILTERSETTINGLINE2 where DocEntry=@DocEntry", CommandType.Text, list.ToArray()) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			if (filter == "FilterSettingLine")
			{
				List<SqlParameter> list2 = new List<SqlParameter>();
				list2.Add(new SqlParameter("@docEntry", Function.ParseInt(dataTable.Rows[0][0])));
				list2.Add(new SqlParameter("@lineNum", Function.ParseInt(dataTable.Rows[0][1])));
				if (ExecuteNonQuery("delete APZ_FILTERSETTINGLINE where DocEntry=@docEntry and LineNum=@lineNum", CommandType.Text, list2.ToArray()) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			if (filter == "FilterSettingTable")
			{
				List<SqlParameter> list3 = new List<SqlParameter>();
				list3.Add(new SqlParameter("@docEntry", Function.ToString(dataTable.Rows[0][0])));
				if (ExecuteNonQuery("delete APZ_FILTERSETTINGLINE2 where DocEntry=@docEntry", CommandType.Text, list3.ToArray()) > 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			if (filter == "FilterSettingChangeSourceType")
			{
				CreateTempTable(dataTable, tempTable);
				BulkCopy(dataTable, tempTable);
				int num = Function.ParseInt(dataTable.Rows[0][0]);
				string query = "delete APZ_FILTERSETTINGLINE where DocEntry=@docEntry ; delete APZ_FILTERSETTINGLINE2 where DocEntry=@docEntry";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@docEntry", num)) > 0)
				{
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, "Fail");
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetFilterLineStructure()
	{
		try
		{
			string query = "select top 0 * from APZ_FILTERSETTINGLINE;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Structure  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetFilterTableStructure()
	{
		try
		{
			string query = "select top 0 * from APZ_FILTERSETTINGLINE2;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Structure  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
