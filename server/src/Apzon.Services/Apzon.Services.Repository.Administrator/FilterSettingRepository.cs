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
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class FilterSettingRepository : BaseService<DataTable>, IFilterSetting
{
	public FilterSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetCategory()
	{
		try
		{
			return ExecuteData("apz_filter_getcategory", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetFilter(string category)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Category", category);
			return ExecuteData("apz_filter_getfilter", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetFilterByCode(string code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Code", code);
			return ExecuteDataTable("apz_filter_getbycode", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataFilter(string datafilter, string userCode)
	{
		try
		{
			List<DatabaseParamater> list = Function.ParseSql(datafilter);
			if (list == null || list.Count == 0)
			{
				return ExecuteDataTable(datafilter, CommandType.Text);
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Para", typeof(string));
			dataTable.Columns.Add("Column", typeof(string));
			dataTable.Columns.Add("Table", typeof(string));
			dataTable.BeginLoadData();
			for (int i = 0; i < list.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Para"] = list[i].ParaName;
				string text = (string)(dataRow["Column"] = list[i].ColumnName);
				if (text.LastIndexOf(".", StringComparison.Ordinal) >= 0)
				{
					dataRow["Column"] = text.Substring(text.LastIndexOf(".", StringComparison.Ordinal) + 1);
				}
				dataRow["Table"] = list[i].TableName;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			IUnitOfWork unitOfWork = GetUnitOfWork();
			DataTable infoColum = unitOfWork.CFL.GetInfoColum(dataTable, userCode);
			List<QueryParamater> list2 = new List<QueryParamater>();
			if (infoColum.IsNotNull())
			{
				foreach (DataRow row in infoColum.Rows)
				{
					QueryParamater queryParamater = new QueryParamater();
					queryParamater.ParaName = Function.ToString(row["Para"]);
					queryParamater.DataType = Function.ToString(row["DataType"]);
					queryParamater.Value = GetDefaultSqlValue(queryParamater.DataType);
					list2.Add(queryParamater);
				}
			}
			DataSet dataSet = unitOfWork.Commons.ExecuteSql(datafilter, list2);
			if (dataSet != null && dataSet.Tables.Count > 1)
			{
				DataTable dataTable2 = dataSet.Tables["Message"];
				if (dataTable2.IsNotNull())
				{
					string text2 = Function.ToString(dataTable2.Rows[0]["MsgType"]);
					string text3 = Function.ToString(dataTable2.Rows[0]["Message"]);
					if (text2 == "5xx")
					{
						Logging.Write("ERROR", text3);
					}
				}
				return dataSet.Tables[1];
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data not found");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult Save(DataTable dt)
	{
		try
		{
			string query = "apz_filter_save";
			if (dt.IsNotNull())
			{
				List<SqlParameter> list = new List<SqlParameter>();
				DataRow dataRow = dt.Rows[0];
				if (dataRow.IsExistsColumn("Code"))
				{
					list.Add(new SqlParameter("@Code", Function.ToString(dataRow["Code"])));
				}
				if (dataRow.IsExistsColumn("Name"))
				{
					list.Add(new SqlParameter("@Name", Function.ToString(dataRow["Name"])));
				}
				if (dataRow.IsExistsColumn("ObjType"))
				{
					list.Add(new SqlParameter("@ObjType", Function.ToString(dataRow["ObjType"])));
				}
				if (dataRow.IsExistsColumn("FilterText"))
				{
					list.Add(new SqlParameter("@FilterText", Function.ToString(dataRow["FilterText"])));
				}
				ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray());
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string SaveUserSign(DataTable dt)
	{
		try
		{
			string query = "DELETE FROM APZ_FILTERASSIGN WHERE \"ObjType\"=@ObjType and \"FilterCode\"=@FilterCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt.Rows[0].IsExistsColumn("ObjType"))
			{
				list.Add(new SqlParameter("@ObjType", Function.ToString(dt.Rows[0]["ObjType"])));
			}
			if (dt.Rows[0].IsExistsColumn("FilterCode"))
			{
				list.Add(new SqlParameter("@FilterCode", Function.ToString(dt.Rows[0]["FilterCode"])));
			}
			ExecuteNonQuery(query, CommandType.Text, list.ToArray());
			query = "INSERT INTO APZ_FILTERASSIGN (\"ObjType\",\"FilterCode\",\"UserName\") VALUES (@ObjType,@FilterCode,@UserName);";
			if (dt.IsNotNull())
			{
				foreach (DataRow row in dt.Rows)
				{
					List<SqlParameter> list2 = new List<SqlParameter>();
					if (row.IsExistsColumn("ObjType"))
					{
						list2.Add(new SqlParameter("@ObjType", Function.ToString(row["ObjType"])));
					}
					if (row.IsExistsColumn("FilterCode"))
					{
						list2.Add(new SqlParameter("@FilterCode", Function.ToString(row["FilterCode"])));
					}
					if (row.IsExistsColumn("UserName"))
					{
						list2.Add(new SqlParameter("@UserName", Function.ToString(row["UserName"])));
					}
					ExecuteNonQuery(query, CommandType.Text, list2.ToArray());
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetUserByFilter(string code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Code", code);
			return ExecuteDataTable("SELECT * FROM APZ_FILTERASSIGN WHERE \"FilterCode\" = @Code;", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveData(string code)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			try
			{
				SqlParameter sqlParameter = new SqlParameter("@Code", code);
				if (ExecuteNonQuery("DELETE APZ_FILTERASSIGN WHERE FilterCode=@Code DELETE APZ_FILTER WHERE Code=@Code", CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return string.Empty;
				}
				dbTransaction.Rollback();
				return Resources.Msg_UnknownError;
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				dbTransaction.Rollback();
				return ex.Message;
			}
		}
	}

	public DataTable GetFilterSelect(string userName, string objType)
	{
		try
		{
			return ExecuteDataTable(parameters: new SqlParameter[2]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@ObjType", objType)
			}, query: GetFilterSelectQuery());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetFilterSelectQuery()
	{
		return "SELECT Code,Name,a.FilterText,a.ObjType FROM dbo.APZ_FILTER a \r\n                                    INNER JOIN dbo.APZ_FilterAssign b ON a.Code=b.FilterCode AND a.ObjType=b.ObjType\r\n                                    WHERE b.UserName=@UserName AND a.ObjType=@ObjType AND a.Valid=1";
	}

	public string GetFilterText(string filter)
	{
		try
		{
			if (!string.IsNullOrEmpty(filter))
			{
				DataTable dataTable = ExecuteDataTable("SELECT isnull(FilterText,'') FROM APZ_FILTER WHERE CODE=@Code", CommandType.Text, new SqlParameter("@Code", filter));
				if (dataTable.IsNotNull())
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public void InsertQueryFilter(string username)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@UserName", username);
			ExecuteNonQuery("apz_filtersetting_insertquery", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
