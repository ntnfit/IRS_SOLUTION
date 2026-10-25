using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class OCashDiscountRepository : BaseService<OCashDiscountsImpl>, IOCashDiscounts, IService<OCashDiscountsImpl>
{
	public OCashDiscountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(OCashDiscountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(OCashDiscountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(OCashDiscountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public OCashDiscountsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable ShowOCashDiscount()
	{
		try
		{
			string query = "SELECT * FROM  OCDC";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetOCashDiscount(string code)
	{
		try
		{
			string query = "SELECT * FROM  OCDC Where Code='" + code + "'";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string SaveDoc(OCashDiscountsImpl doc)
	{
		string empty = string.Empty;
		try
		{
			bool flag = Check(doc.Code);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE OCDC  SET TableDesc=N'{doc.TableDesc}'\r\n                                                     WHERE Code=N'{doc.Code}'" : $"INSERT INTO OCDC  (Code , TableDesc)\r\n                                                     VALUES (N'{doc.Code}', N'{doc.TableDesc}')");
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

	public string DeleteDoc(string code)
	{
		try
		{
			string query = " delete OCDC where Code = '" + code + "'";
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

	public bool Check(string code)
	{
		try
		{
			string query = "SELECT Code \r\n\t\t\t\t\t\t\t        FROM  OCDC\r\n                                    WHERE Code = '" + code + "'";
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
}
