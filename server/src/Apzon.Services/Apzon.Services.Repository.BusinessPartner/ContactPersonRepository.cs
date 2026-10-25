using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class ContactPersonRepository : BaseService<ContactPersonImpl>, IContactPersons, IService<ContactPersonImpl>
{
	public ContactPersonRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(ContactPersonImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(ContactPersonImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(ContactPersonImpl entity)
	{
		throw new NotImplementedException();
	}

	public ContactPersonImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable ShowContactPerson(string code)
	{
		try
		{
			return ExecuteDataTable(ShowContactPersonQuery(), CommandType.Text, new SqlParameter("@Code", code ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowContactPerson  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string ShowContactPersonQuery()
	{
		return "SELECT *\r\n                              FROM [dbo].[OCPR] where ((CardCode=@Code and @Code != '') OR @Code = '')";
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(CntctCode),0) as MaxCode\r\n                                    FROM ocpr ";
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

	public string SaveDoc(ContactPersonImpl doc)
	{
		string empty = string.Empty;
		try
		{
			bool flag = Check(doc.CntctCode);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE ocpr  SET \r\n                                                     CardCode=N'{doc.CardCode}',Name=N'{doc.Name}', Title=N'{doc.Title}',Position=N'{doc.Position}'\r\n                                                     ,Address=N'{doc.Address}',Tel1=N'{doc.Tel1}',Tel2=N'{doc.Tel2}',Cellolar=N'{doc.Cellolar}'\r\n                                                        ,Fax=N'{doc.Fax}',E_MailL=N'{doc.E_MailL}'\r\n                                                       ,Pager=N'{doc.Pager}',Notes1=N'{doc.Notes1}',Notes2=N'{doc.Notes2}',Password=N'{doc.Password}'\r\n                                                        ,UserSign={doc.UserSign},BirthPlace=N'{doc.BirthPlace}',BirthDate=N'{doc.BirthDate}'\r\n                                                        ,Gender=N'{doc.Gender}',Profession=N'{doc.Profession}',BirthCity=N'{doc.BirthCity}',Active=N'{doc.Active}'\r\n                                                       WHERE CntctCode={doc.CntctCode}" : $"INSERT INTO ocpr  (CntctCode , CardCode,Name, Title,Position\r\n                                                     ,Address,Tel1,Tel2,Cellolar,Fax,E_MailL\r\n                                                       ,Pager,Notes1,Notes2,Password,UserSign,BirthPlace,BirthDate\r\n                                                        ,Gender,Profession,BirthCity,Active)\r\n                                                     VALUES ({doc.CntctCode}, N'{doc.CardCode}', N'{doc.Name}', N'{doc.Title}', N'{doc.Position}', N'{doc.Address}',\r\n                                                        N'{doc.Tel1}', N'{doc.Tel2}', N'{doc.Cellolar}', N'{doc.Fax}', N'{doc.E_MailL}', N'{doc.Pager}',\r\n                                                        N'{doc.Notes1}', N'{doc.Notes2}', N'{doc.Password}', {doc.UserSign}, N'{doc.BirthPlace}', N'{doc.BirthDate}',\r\n                                                        N'{doc.Gender}', N'{doc.Profession}', N'{doc.BirthCity}', N'{doc.Active}')");
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
			string query = " delete ocpr where CntctCode = " + code;
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
			string query = "SELECT CntctCode \r\n\t\t\t\t\t\t\t        FROM  ocpr\r\n                                    WHERE CntctCode = " + code;
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
