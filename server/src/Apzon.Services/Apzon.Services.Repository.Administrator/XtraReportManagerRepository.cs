using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator;

namespace Apzon.Services.Repository.Administrator;

public class XtraReportManagerRepository : BaseService<DataTable>, IXtraReportManager
{
	public XtraReportManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet LoadReport(string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (branch > 0)
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData("Apz_XtraReport_LoadReport", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadLayout(string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (branch > 0)
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData("Apz_XtraReport_LoadLayout", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult SaveReport(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				int num = SaveData(dt);
				if (num > 0)
				{
					dbTransaction.Commit();
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
				dbTransaction.Rollback();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Save Error!");
	}

	private int SaveData(DataTable dt)
	{
		try
		{
			string text = "";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt.IsNotNull())
			{
				int num = Function.ParseInt(dt.Rows[0]["ID"]);
				text = GetQuerySaveReport(num <= 0);
				list.Add(new SqlParameter("@TypeCode", Function.ToString(dt.Rows[0]["TypeCode"])));
				list.Add(new SqlParameter("@TypeName", Function.ToString(dt.Rows[0]["TypeName"])));
				list.Add(new SqlParameter("@DocCode", Function.ToString(dt.Rows[0]["DocCode"])));
				list.Add(new SqlParameter("@DocName", Function.ToString(dt.Rows[0]["DocName"])));
				list.Add(new SqlParameter("@Menu", Function.ToString(dt.Rows[0]["Menu"])));
				list.Add(new SqlParameter("@CreateDate", Function.ParseDateTime(dt.Rows[0]["CreateDate"])));
				list.Add(new SqlParameter("@UserSign", Function.ToString(dt.Rows[0]["UserSign"])));
				list.Add(new SqlParameter("@Status", Function.ToString(dt.Rows[0]["Status"])));
				list.Add(new SqlParameter("@Description", Function.ToString(dt.Rows[0]["Description"])));
				list.Add(new SqlParameter("@Text", Function.ToString(dt.Rows[0]["Text"])));
				if (num > 0)
				{
					list.Add(new SqlParameter("@ID", num));
				}
				DataTable dataTable = ExecuteDataTable(text, CommandType.Text, list.ToArray());
				if (dataTable.IsNotNull())
				{
					return Function.ParseInt(dataTable.Rows[0][0]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	protected virtual string GetQuerySaveReport(bool insert)
	{
		if (insert)
		{
			return "INSERT INTO dbo.APZ_RDOC (TypeCode, TypeName, DocCode, DocName, Menu, CreateDate, UserSign, Status, Description, Text)\r\nVALUES (@TypeCode,@TypeName,@DocCode,@DocName,@Menu,@CreateDate,@UserSign,@Status,@Description,@Text)\r\nSELECT SCOPE_IDENTITY()";
		}
		return "UPDATE dbo.APZ_RDOC SET TypeCode = @TypeCode,TypeName = @TypeName,DocCode = @DocCode,Menu = @Menu,UpdateDate = @UpdateDate,UserSign2 =  @UserSign2,Status = @Status,Description = @Description,Text = @Text\r\nOUTPUT Inserted.ID\r\nWHERE ID =@ID";
	}

	public string GetDataReportLayoutById(int id)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_XtraReportManager_GetDataReportLayoutById", CommandType.StoredProcedure, new SqlParameter("@id", id));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public string RemoveReport(int id)
	{
		try
		{
			string query = "DELETE FROM APZ_RDOC WHERE ID = @id";
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@id", id)) > 0)
			{
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}
}
