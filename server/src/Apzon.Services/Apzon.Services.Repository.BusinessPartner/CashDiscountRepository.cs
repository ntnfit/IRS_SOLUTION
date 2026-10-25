using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class CashDiscountRepository : BaseService<CashDiscountsImpl>, ICashDiscounts, IService<CashDiscountsImpl>
{
	public CashDiscountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(CashDiscountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(CashDiscountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(CashDiscountsImpl entity)
	{
		throw new NotImplementedException();
	}

	public CashDiscountsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public int GetMaxLineId()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(LineId),0) as ML\r\n                                    FROM CDC1 ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["ML"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public DataTable ShowCashDiscount()
	{
		try
		{
			string query = "SELECT * FROM  CDC1";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetCashDiscount(string code)
	{
		try
		{
			string query = "SELECT * FROM  CDC1 Where CdcCode='" + code + "'";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string SaveDoc(CashDiscountsImpl doc)
	{
		string empty = string.Empty;
		new List<SqlParameter>();
		try
		{
			bool flag = Check(doc.CdcCode, doc.LineId);
			string empty2 = string.Empty;
			empty2 = ((flag || !(doc.Discount != 0m)) ? $"UPDATE CDC1  SET NumOfDays={doc.NumOfDays}, Discount={doc.Discount}\r\n                                                     WHERE CdcCode=N'{doc.CdcCode}' And LineId={doc.LineId}" : $"INSERT INTO CDC1  (CdcCode , LineId, NumOfDays , Discount)\r\n                                                     VALUES (N'{doc.CdcCode}', {doc.LineId}, {doc.NumOfDays}, {doc.Discount})");
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
			string query = " delete CDC1 where CdcCode = '" + code + "'";
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

	public bool Check(string code, int lineId)
	{
		try
		{
			string query = "SELECT CdcCode \r\n\t\t\t\t\t\t\t        FROM  CDC1\r\n                                    WHERE CdcCode = '" + code + "' and LineId=" + lineId;
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
