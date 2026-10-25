using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class DepartmentRepository : BaseService<DepartmentImpl>, IDepartment, IService<DepartmentImpl>
{
	public DepartmentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable ShowDepartment()
	{
		try
		{
			string query = ShowDepartmentQuery();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string ShowDepartmentQuery()
	{
		return "select Code,Name,ISNULL( Remarks,'') as Remarks from OUDP";
	}

	public DataTable Search(DepartmentImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "select Code,Name,ISNULL( Remarks,'') as Remarks from OUDP  \r\n                                Where 1=1";
			if (!string.IsNullOrEmpty(doc.Name))
			{
				text = text + " AND Name like N'%" + doc.Name + "%' ";
			}
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Location Department - Doc ", ex.Message);
		}
		return result;
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(Code ),0) as Code\r\n                            FROM OUDP ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["Code"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public bool Check(int code)
	{
		try
		{
			string query = "SELECT   A.Code \r\n                            FROM OUDP A \r\n                            WHERE A.Code = '" + code + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
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

	public string SaveDoc(DepartmentImpl doc)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				bool flag = Check(doc.Code);
				string empty2 = string.Empty;
				if (!flag)
				{
					empty2 = "INSERT INTO OUDP  ( Code,Name,Remarks,UserSign)\r\n                                                     VALUES \r\n                                        ( ";
					empty2 += " @code ";
					list.Add(new SqlParameter("@code", Function.ParseInt(doc.Code)));
					empty2 += ", @name ";
					list.Add(new SqlParameter("@name", Function.ToString(doc.Name)));
					empty2 += ", @remarks ";
					list.Add(new SqlParameter("@remarks", Function.ToString(doc.Remarks)));
					empty2 += ", @userSign ";
					list.Add(new SqlParameter("@userSign", Function.ToString(doc.UserSign)));
					empty2 += " )";
				}
				else
				{
					list = new List<SqlParameter>();
					empty2 = "UPDATE OUDP  \r\n                            SET ";
					empty2 += "Name= @name ";
					list.Add(new SqlParameter("@name", Function.ToString(doc.Name)));
					empty2 += ",Remarks= @remarks ";
					list.Add(new SqlParameter("@remarks", Function.ToString(doc.Remarks)));
					empty2 += ",UserSign= @userSign ";
					list.Add(new SqlParameter("@userSign", Function.ToString(doc.UserSign)));
					empty2 += " WHERE Code =@code";
					list.Add(new SqlParameter("@code", Function.ParseInt(doc.Code)));
				}
				if (ExecuteNonQuery(empty2, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
				return "ERROR";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
				empty = ex.Message;
			}
			return empty;
		}
	}

	public string DeleteDoc(int code)
	{
		try
		{
			string query = " delete OUDP where Code = '" + code + "'";
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public string Insert(DepartmentImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(DepartmentImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(DepartmentImpl entity)
	{
		throw new NotImplementedException();
	}

	public DepartmentImpl GetById(int id)
	{
		throw new NotImplementedException();
	}
}
