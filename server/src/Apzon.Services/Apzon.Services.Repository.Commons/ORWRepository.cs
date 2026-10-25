using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class ORWRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IORW
{
	private readonly string _tempOudo = "#OUDO";

	private readonly string _tempUdo1 = "#UDO1";

	private readonly string _tempUdo2 = "#UDO2";

	private readonly string _tempUdo3 = "#UDO3";

	private readonly string _tempUdo4 = "#UDO4";

	private string Temp_OUTBChild = "#OUTBchild";

	private string Temp_OUTBParent = "#OUTBparent";

	public DataTable GetTableNameOUDO()
	{
		try
		{
			return ExecuteDataTable("select * from OUDO;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableNameOUDO ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetTableColumNameUDO2(string column)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@column", column);
			return ExecuteDataTable("select DISTINCT COLUMN_NAME, (case when COLUMN_NAME IN ('Code','Name','DocNum','CreateDate','UpdateDate') then  'Y' else 'N' end) as IsCheck from information_schema.columns where table_name = '@' + @column", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableColumNameUDO2 ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetTableColumNameUDO3(string tableName)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@TblName", "@" + tableName);
			return ExecuteDataTable("select DISTINCT COLUMN_NAME,CASE WHEN ISNULL(T1.Descr,'')='' THEN T0.COLUMN_NAME ELSE T1.Descr END Dscription,\r\n                                         (case when COLUMN_NAME IN ('Code','DocEntry') then  'Y' else 'N' end) as IsCheckColumn,(case when COLUMN_NAME IN ('Code') then  'Y' else 'N' end) as ColEdit \r\n                                        FROM information_schema.columns T0 LEFT JOIN CUFD T1 ON T0.TABLE_NAME=T1.TableID AND T0.COLUMN_NAME='U_'+T1.AliasID  where table_name = @TblName", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableColumNameUDO3 ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetTableChild(int objtype, string usedInObj)
	{
		try
		{
			return ExecuteDataTable("Apz_udo_GetTableUDO4", CommandType.StoredProcedure, new SqlParameter("@type", objtype), new SqlParameter("@usedInObj", usedInObj));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableChild ", ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetTableColumnChildQuery()
	{
		return "select DISTINCT A.COLUMN_NAME,CASE WHEN ISNULL(T1.Descr,'')='' THEN A.COLUMN_NAME ELSE T1.Descr END Dscription, \r\n                     CASE when isnull(b.ColEdit,'N') <> 'Y' then 'N' else 'Y' end ColEdit,case when isnull(b.ColIsUsed,'N') <> 'Y' then 'N' else 'Y' end ColIsUsed, '' as LogName  \r\n                     from INFORMATION_SCHEMA.COLUMNS A LEFT JOIN CUFD T1 ON A.TABLE_NAME=T1.TableID AND A.COLUMN_NAME='U_'+T1.AliasID \r\n                     INNER JOIN OUTB D ON  substring(A.TABLE_NAME,2,19) =  D.TableName\r\n                     LEFT JOIN UDO4 B ON A.COLUMN_NAME=B.ColAlias and B.Code = D.UsedInObj \r\n                     WHERE TABLE_NAME = '@' + @column";
	}

	public DataTable GetTableColumChild(string colchild)
	{
		try
		{
			string tableColumnChildQuery = GetTableColumnChildQuery();
			SqlParameter sqlParameter = new SqlParameter("@column", colchild);
			ExecuteDataTable(tableColumnChildQuery, CommandType.Text, sqlParameter);
			return ExecuteDataTable(tableColumnChildQuery, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableNameChild ", ex.Message);
		}
		return new DataTable();
	}

	public string SaveORWDataSet(DataSet ds, Enums.TransactionType transtype)
	{
		try
		{
			return Process(ApzObjectTypes.oUserObjectsMD, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetDropTableLogQuery(string logTable)
	{
		return string.Format("IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}')\r\n                                    BEGIN\r\n                                        DROP TABLE [{0}]\r\n                                        SELECT 1\r\n                                        RETURN\r\n                                    END\r\n                                    SELECT -1", "@" + logTable);
	}

	protected virtual string GetDropTableQuery(string tableName)
	{
		return $"DROP TABLE \"{tableName}\";";
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		try
		{
			switch (transtype)
			{
			case Enums.TransactionType.D:
			case Enums.TransactionType.C:
			{
				if (string.IsNullOrEmpty(objId))
				{
					return new HttpResult(Enums.MessageCode.Error, "object code is empty");
				}
				DataTable dataTable3 = ExecuteDataTable("SELECT * FROM OUDO WHERE \"Code\" = @Code;", CommandType.Text, new SqlParameter("@Code", objId));
				if (dataTable3.IsNotNull())
				{
					string text2 = Function.ToString(dataTable3.Rows[0]["TableName"]);
					string text3 = Function.ToString(dataTable3.Rows[0]["LogTable"]);
					if (ExecuteNonQuery("UPDATE OUTB SET \"UsedInObj\" = NULL WHERE \"TableName\" = @TableName;", CommandType.Text, new SqlParameter("@TableName", text2)) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "No record affected - update used in object");
					}
					DataTable dataTable4 = ExecuteDataTable("SELECT \"TableName\", \"LogName\" FROM UDO1 WHERE \"Code\" = @Code;", CommandType.Text, new SqlParameter("@Code", objId));
					if (Function.ToString(dataTable3.Rows[0]["CanLog"]) == "Y" && !string.IsNullOrEmpty(text3))
					{
						string dropTableLogQuery = GetDropTableLogQuery(text3);
						if (Function.ParseInt(ExecuteDataTable(dropTableLogQuery, CommandType.Text, new SqlParameter("@Name", "@" + text3)).Rows[0][0]) <= 0)
						{
							return new HttpResult(Enums.MessageCode.Error, "No record affected - drop log table");
						}
						if (dataTable4.IsNotNull())
						{
							for (int j = 0; j < dataTable4.Rows.Count; j++)
							{
								string text4 = Function.ToString(dataTable4.Rows[j]["LogName"]);
								if (!string.IsNullOrEmpty(text4) && Function.ParseInt(ExecuteDataTable(GetDropTableLogQuery(text4), CommandType.Text).Rows[0][0]) <= 0)
								{
									return new HttpResult(Enums.MessageCode.Error, "No record affected -drop log name " + text4);
								}
							}
						}
					}
					if (transtype == Enums.TransactionType.D)
					{
						if (Function.ParseInt(ExecuteDataTable(GetDropTableLogQuery(text2), CommandType.Text).Rows[0][0]) <= 0)
						{
							return new HttpResult(Enums.MessageCode.Error, "No record affected - delete header data");
						}
						if (dataTable4.IsNotNull())
						{
							for (int k = 0; k < dataTable4.Rows.Count; k++)
							{
								string text5 = Function.ToString(dataTable4.Rows[k]["TableName"]);
								if (Function.ParseInt(ExecuteDataTable(GetDropTableLogQuery(text5), CommandType.Text).Rows[0][0]) <= 0)
								{
									return new HttpResult(Enums.MessageCode.Error, "No record affected - delete son " + text5);
								}
								string text6 = Function.ToString(dataTable4.Rows[k]["LogName"]);
								if (!string.IsNullOrEmpty(text6))
								{
									Function.ParseInt(ExecuteDataTable(GetDropTableLogQuery(text6), CommandType.Text).Rows[0][0]);
								}
							}
						}
					}
					if (ExecuteNonQuery("DELETE FROM OUDO WHERE \"Code\" = @Code; \r\n                                                            DELETE FROM UDO1 WHERE \"Code\" = @Code;\r\n                                                            DELETE FROM UDO2 WHERE \"Code\"= @Code;\r\n                                                            DELETE FROM UDO3 WHERE \"Code\"= @Code;\r\n                                                            DELETE FROM UDO4 WHERE \"Code\"= @Code;", CommandType.Text, new SqlParameter("@Code", objId)) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "No record affected - unregister object \"" + objId + "\"");
					}
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, Messages.D404);
			}
			case Enums.TransactionType.A:
			case Enums.TransactionType.U:
			{
				HttpResult httpResult = new HttpResult();
				if (ds == null || ds.Tables.Count < 4 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
				{
					httpResult.Message = "No data to save";
				}
				else
				{
					if (transtype == Enums.TransactionType.A)
					{
						object value = ds.Tables["dtOUDO"].Rows[0]["Code"];
						string query = "select 1 from oudo where \"code\" = @Code;";
						if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Code", value)).IsNotNull())
						{
							throw new Exception("Unique ID already exists");
						}
					}
					CreateTempTable(ds.Tables["dtOUDO"], _tempOudo);
					BulkCopy(ds.Tables["dtOUDO"], _tempOudo);
					CreateTempTable(ds.Tables["dtODU1"], _tempUdo1);
					BulkCopy(ds.Tables["dtODU1"], _tempUdo1);
					CreateTempTable(ds.Tables["dtODU2"], _tempUdo2);
					BulkCopy(ds.Tables["dtODU2"], _tempUdo2);
					CreateTempTable(ds.Tables["dtODU3"], _tempUdo3);
					BulkCopy(ds.Tables["dtODU3"], _tempUdo3);
					CreateTempTable(ds.Tables["dtODU4"], _tempUdo4);
					BulkCopy(ds.Tables["dtODU4"], _tempUdo4);
					DataTable dataTable = ds.Tables["dtOUDO"];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
					}
					DataTable dataTable2 = ExecuteDataTable("apz_udo_SaveprocessUDO", CommandType.StoredProcedure);
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						httpResult.Message = Function.ToString(dataTable2.Rows[0]["ErrorMessage"]);
						string text = Function.ToString(dataTable2.Rows[0]["ErrorNumber"]);
						httpResult.Message = text + " - " + httpResult.Message;
					}
					else
					{
						httpResult.Message = "";
					}
				}
				return httpResult;
			}
			default:
				return new HttpResult();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string RemoveObjectRegistration(string code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "Apz_udo_RemoveObjectRegistration";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@code", code));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No rows deleted";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteAllORW  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetColEditTableUDO2Query()
	{
		return "select A.COLUMN_NAME, case when B.Code is null then 'N' else 'Y' end IsCheck from INFORMATION_SCHEMA.COLUMNS A \r\n\t\t\t\t\t  INNER JOIN OUDO C on A.TABLE_NAME='@'+C.TableName\r\n\t\t\t\t\t  LEFT JOIN UDO2 B ON A.COLUMN_NAME=B.ColAlias and  C.Code=B.Code  WHERE C.Code = @column;";
	}

	public DataTable GetcolEditTableUDO2(string colUDO2)
	{
		try
		{
			string colEditTableUDO2Query = GetColEditTableUDO2Query();
			SqlParameter sqlParameter = new SqlParameter("@column", colUDO2);
			return ExecuteDataTable(colEditTableUDO2Query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableNameChild ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetcolEditTableUDO3(string colUDO3)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@column", colUDO3);
			return ExecuteDataTable("select A.COLUMN_NAME, case when B.Code is null then 'N' else 'Y' end IsCheckColumn,ColEdit from INFORMATION_SCHEMA.COLUMNS A \r\n\t\t\t\t\t\t\t\t\t  INNER JOIN OUDO C on A.TABLE_NAME='@'+C.TableName\r\n\t\t\t\t\t\t\t\t\t  LEFT JOIN UDO3 B ON A.COLUMN_NAME=B.ColAlias and  C.Code=B.Code\r\n\t\t\t\t\t\t\t\t\t  WHERE C.Code= @column", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableNameChild ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetcolEditTableUDO4(string colUDO4)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@column", colUDO4);
			return ExecuteDataTable("select DISTINCT A.COLUMN_NAME, B.ColIsUsed,B.ColEdit from INFORMATION_SCHEMA.COLUMNS A \r\n\t\t\t\t\t\t\t\t\t  INNer JOIN UDO4 B ON A.COLUMN_NAME=B.ColAlias \r\n\t\t\t\t\t\t\t\t\t  WHERE B.Code= @column", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableNameChild ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetcolEditTableUDO1(int objtype, string colUDO1)
	{
		try
		{
			return ExecuteDataTable("Apz_udo_GetTableUDO1", CommandType.StoredProcedure, new SqlParameter("@type", objtype), new SqlParameter("@colUDO1", colUDO1));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetTableNameChild ", ex.Message);
		}
		return new DataTable();
	}

	private string InsertData(DataRow dataRow)
	{
		try
		{
			string text = "";
			string text2 = "dbo.[@" + Function.ToString(dataRow["LogTable"]) + "] ";
			switch (Function.ParseInt(dataRow["Type"]))
			{
			case 1:
				text = text + "CREATE TABLE " + text2 + " ([Code] [NVARCHAR](30) NOT NULL,[Name] [NVARCHAR](100) NULL,[DocEntry] [INT] NOT NULL,[Canceled] [CHAR](1) NULL,[Object] [NVARCHAR](20) NULL,[LogInst] [INT] NOT NULL,[UserSign] [INT] NULL,[Transfered] [CHAR](1) NULL,[CreateDate] [DATETIME] NULL,[CreateTime] [SMALLINT] NULL,[UpdateDate] [DATETIME] NULL,[UpdateTime] [SMALLINT] NULL,[DataSource] [CHAR](1) NULL, PRIMARY KEY (Code, LogInst))";
				break;
			case 3:
				text = text + "CREATE TABLE " + text2 + " ([DocEntry] [INT] NOT NULL,[DocNum] [INT] NULL,[Period] [INT] NULL,[Instance] [SMALLINT] NULL,[Series] [INT] NULL,[Handwrtten] [CHAR](1) NULL,[Canceled] [CHAR](1) NULL,[Object] [NVARCHAR](20) NULL,[LogInst] [INT] NOT NULL,[UserSign] [INT] NULL,[Transfered] [CHAR](1) NULL,[Status] [CHAR](1) NULL,[CreateDate] [DATETIME] NULL,[CreateTime] [SMALLINT] NULL,[UpdateDate] [DATETIME] NULL,[UpdateTime] [SMALLINT] NULL,[DataSource] [CHAR](1) NULL,[RequestStatus] [CHAR](1) NULL,[Creator] [NVARCHAR](8) NULL,[Remark] [NTEXT] NULL,PRIMARY KEY (DocEntry, LogInst))";
				break;
			}
			ExecuteNonQuery(text, CommandType.Text);
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDT(string tableName)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '@" + tableName + "') \r\nBEGIN\r\nDROP TABLE dbo.[@" + tableName + "]\r\nEND";
				if (ExecuteNonQuery(query, CommandType.Text) < 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return "No record effected";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataSet GetFieldsForFind(string tableName, string type)
	{
		try
		{
			return ExecuteData("APZ_UDO_GetFieldsForFind", CommandType.StoredProcedure, new SqlParameter("@tableName", tableName), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_UDO_GetFieldsForFind ", ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetFieldsForForm(string tableName, string type)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[2]
			{
				new SqlParameter("@tableName", tableName),
				new SqlParameter("@type", type)
			};
			ExecuteData("APZ_UDO_GetFieldsForForm", CommandType.StoredProcedure, parameters);
			return ExecuteData("APZ_UDO_GetFieldsForForm", CommandType.StoredProcedure, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_UDO_GetFieldsForForm ", ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetFieldsChild(string tableName)
	{
		try
		{
			return ExecuteData("APZ_UDO_GetFieldsChild", CommandType.StoredProcedure, new SqlParameter("@tableName", tableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "APZ_UDO_GetFieldsChild ", ex.Message);
		}
		return new DataSet();
	}

	public HttpResult Unregister(string code)
	{
		return Process(ApzObjectTypes.oUserObjectsMD, -1, Enums.TransactionType.C, new DataSet(), code);
	}

	public HttpResult Delete(string code)
	{
		return Process(ApzObjectTypes.oUserObjectsMD, -1, Enums.TransactionType.D, new DataSet(), code);
	}
}
