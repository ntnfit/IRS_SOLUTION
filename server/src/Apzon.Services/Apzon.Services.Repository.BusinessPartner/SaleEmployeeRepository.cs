using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class SaleEmployeeRepository : BaseService<SaleEmployeesImpl>, ISaleEmployees, IService<SaleEmployeesImpl>
{
	public SaleEmployeeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(SaleEmployeesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(SaleEmployeesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(SaleEmployeesImpl entity)
	{
		throw new NotImplementedException();
	}

	public SaleEmployeesImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(GroupCode),0) as CRDMGRPC\r\n                                    FROM OCRG ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["CRDMGRPC"]);
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	public DataTable ShowSaleEmployee()
	{
		try
		{
			string query = "apz_Employee_ShowSaleEmployee";
			return ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	public string SaveDoc(SaleEmployeesImpl doc)
	{
		string empty = string.Empty;
		new List<SqlParameter>();
		try
		{
			bool flag = Check(doc.SlpCode);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE OSLP  SET SlpName=N'{doc.SlpName}', Memo=N'{doc.Memo}'\r\n                                                       , Commission={doc.Commission}, UAddress=N'{doc.UAddress}'\r\n                                                     WHERE SlpCode={doc.SlpCode}" : $"INSERT INTO OSLP  (SlpCode , SlpName,Memo, Commission , UAddress)\r\n                                                     VALUES ({doc.SlpCode}, N'{doc.SlpName}', N'{doc.Memo}', {doc.Commission}, N'{doc.UAddress}' )");
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int code)
	{
		try
		{
			string query = " delete OSLP where SlpCode = " + code;
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

	public bool Check(int code)
	{
		try
		{
			string query = "SELECT SlpCode \r\n\t\t\t\t\t\t\t        FROM  OSLP\r\n                                    WHERE SlpCode = " + code;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
