using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Integrated;

namespace Apzon.Services.Repository.Integrated;

public class IntegratedRepository : BaseService<DataTable>, IIntegrated
{
	public IntegratedRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string CheckIRSAuthorizations()
	{
		string result = "";
		try
		{
			string query = "APZ_Integrated_CheckIRSAuthorizations";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable Search(DataTable doc)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Integrated_Search";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@ObjectType", Function.ToString(doc.Rows[0]["ObjectType"])),
				new SqlParameter("@FromDate", Function.ParseDateTime(doc.Rows[0]["FromDate"], "yyyyMMdd")),
				new SqlParameter("@ToDate", Function.ParseDateTime(doc.Rows[0]["ToDate"], "yyyyMMdd")),
				new SqlParameter("@IsIns", Function.ToString(doc.Rows[0]["IsIns"])),
				new SqlParameter("@IsReceiptFromProduct", Function.ToString(doc.Rows[0]["IsReceiptFromProduct"])),
				new SqlParameter("@IsImported", Function.ToString(doc.Rows[0]["IsImported"]))
			};
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable SearchLine(int objType, string id, string isIns, string isRecFromProd)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Integrated_SearchLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ObjectType", objType));
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@IsIns", isIns));
			list.Add(new SqlParameter("@IsReceiptFromProduct", isRecFromProd));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable SearchLineForSave(int objType, string id, string isIns, string isRecFromProd)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Integrated_SearchLineForSave";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ObjectType", objType));
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@IsIns", isIns));
			list.Add(new SqlParameter("@IsReceiptFromProduct", isRecFromProd));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string SaveADoc(DocumentsImpl doc)
	{
		return "";
	}

	public DataTable GetListConfig()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Integrated_GetListConfig";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CheckExists(DataTable dt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Integrated_CheckExists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ObjectType", Function.ParseInt(dt.Rows[0]["ObjectType"])));
			list.Add(new SqlParameter("@IsIns", Function.ToString(dt.Rows[0]["IsIns"])));
			list.Add(new SqlParameter("@IsReceiptFromProduct", Function.ToString(dt.Rows[0]["IsReceiptFromProduct"])));
			list.Add(new SqlParameter("@ID", Function.ToString(dt.Rows[0]["ID"])));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetDocGridStruct()
	{
		DataSet result = new DataSet();
		try
		{
			result = ExecuteData("APZ_Integrated_GetDocGridStruct", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet GetDocLineGridStruct()
	{
		DataSet result = new DataSet();
		try
		{
			result = ExecuteData("APZ_Integrated_GetDocLineGridStruct", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string MapCodeSaveADoc(DataTable dt)
	{
		string result = "ERROR";
		try
		{
			DataRow dataRow = dt.Rows[0];
			List<SqlParameter> list = new List<SqlParameter>();
			string empty = string.Empty;
			if (!MapCodeCheckDoc(Function.ParseInt(dataRow["TypeId"]), Function.ToString(dataRow["Code"])))
			{
				empty = "INSERT INTO [POSS].[dbo].MAP_CODE\r\n                                   ([TypeId]\r\n                                   ,[TypeName]\r\n                                   ,[Code]\r\n                                   ,[Name]\r\n                                   ,[NewCode]\r\n                                   ,[NewName]) VALUES (";
				if (Function.ParseInt(dataRow["TypeId"]) > 0)
				{
					empty += "@TypeId ";
					list.Add(new SqlParameter("@TypeId", Function.ParseInt(dataRow["TypeId"])));
				}
				else
				{
					empty += "0 ";
				}
				empty += ",@TypeName ";
				list.Add(new SqlParameter("@TypeName", Function.ToString(dataRow["TypeName"])));
				empty += ",@Code ";
				list.Add(new SqlParameter("@Code", Function.ToString(dataRow["Code"])));
				empty += ",@Name ";
				list.Add(new SqlParameter("@Name", Function.ToString(dataRow["Name"])));
				empty += ",@NewCode ";
				list.Add(new SqlParameter("@NewCode", Function.ToString(dataRow["NewCode"])));
				empty += ",@NewName ";
				list.Add(new SqlParameter("@NewName", Function.ToString(dataRow["NewName"])));
				empty += " )";
			}
			else
			{
				list = new List<SqlParameter>();
				empty = "Update [POSS].[dbo].MAP_CODE SET ";
				empty += " TypeName=@TypeName ";
				list.Add(new SqlParameter("@TypeName", Function.ToString(dataRow["TypeName"])));
				empty += " ,Name=@Name ";
				list.Add(new SqlParameter("@Name", Function.ToString(dataRow["Name"])));
				empty += " ,NewCode=@NewCode ";
				list.Add(new SqlParameter("@SrcPrice", Function.ToString(dataRow["NewCode"])));
				empty += " ,NewName=@NewName ";
				list.Add(new SqlParameter("@NewName", Function.ToString(dataRow["NewName"])));
				empty += " WHERE ";
				if (Function.ParseInt(dataRow["TypeId"]) > 0)
				{
					empty += "TypeId=@TypeId ";
					list.Add(new SqlParameter("@TypeId", Function.ParseInt(dataRow["TypeId"])));
				}
				else
				{
					empty += "TypeId=0 ";
				}
				empty += " and Code=@Code ";
				list.Add(new SqlParameter("@Code", Function.ToString(dataRow["Code"])));
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					result = "";
				}
				else
				{
					dbTransaction.Rollback();
					result = "ERROR";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE A DOC ITM1  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public bool MapCodeCheckDoc(int TypeId, string Code)
	{
		try
		{
			string query = "SELECT count(*) FROM [POSS].[dbo].MAP_CODE  WHERE TypeId = @ListNum and Code=@Code";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@TypeId", TypeId));
			list.Add(new SqlParameter("@Code", Code));
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
}
