using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class CreditCardTypeRepository : BaseService<CreditCardTypesImpl>, ICreditCardTypes, IService<CreditCardTypesImpl>
{
	public CreditCardTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(CreditCardTypesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(CreditCardTypesImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(CreditCardTypesImpl entity)
	{
		throw new NotImplementedException();
	}

	public CreditCardTypesImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable ShowCreditCardType()
	{
		try
		{
			string query = "SELECT * FROM  OCRC";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string SaveDoc(CreditCardTypesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			bool flag = Check(doc.CreditCard);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE OCRC  SET CardName=N'{doc.CardName}', CompanyId=N'{doc.CompanyId}' , Phone=N'{doc.Phone}' \r\n                                                            , IntTaxCode=N'{doc.IntTaxCode}', Locked=N'{doc.Locked}'\r\n                                                     WHERE CreditCard={doc.CreditCard}" : $"INSERT INTO OCRC  (CreditCard , CardName,CompanyId, Phone,\r\n                                                                IntTaxCode,Locked)\r\n                                                     VALUES ({doc.CreditCard}, N'{doc.CardName}', N'{doc.CompanyId}', N'{doc.Phone}', N'{doc.IntTaxCode}' , N'{doc.Locked}')");
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
			string query = " delete OCRC where CreditCard = " + code;
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

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(CreditCard),0) as MaxCode\r\n                                    FROM OCRC ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["MaxCode"]);
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
			string query = "SELECT CreditCard \r\n\t\t\t\t\t\t\t        FROM  OCRC\r\n                                    WHERE CreditCard = " + code;
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
