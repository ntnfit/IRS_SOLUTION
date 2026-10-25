using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Services.Interfaces.BusinessPartner;

namespace Apzon.Services.Repository.BusinessPartner;

public class CardGroupRepository : BaseService<CardGroupsImpl>, ICardGroups, IService<CardGroupsImpl>
{
	public CardGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Insert(CardGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(CardGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(CardGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public CardGroupsImpl GetById(int id)
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public DataTable ShowCardGroup()
	{
		try
		{
			string query = "SELECT \"GroupCode\",\"GroupName\",\"GroupType\",\"Locked\",\r\n                                    \"UserSign\"\r\n\t\t\t\t\t\t\t   FROM  OCRG;";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string SaveDoc(CardGroupsImpl doc)
	{
		string empty = string.Empty;
		new List<SqlParameter>();
		try
		{
			bool flag = Check(doc.GroupCode);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE OCRG  SET GroupName=N'{doc.GroupName}', GroupType=N'{doc.GroupType}'\r\n                                                       , Locked=N'{doc.Locked}', UserSign={doc.UserSign}\r\n                                                     WHERE GroupCode={doc.GroupCode}" : $"INSERT INTO OCRG  (GroupCode , GroupName,GroupType, Locked ,UserSign)\r\n                                                     VALUES ({doc.GroupCode}, '{doc.GroupName}', N'{doc.GroupType}', N'{doc.Locked}', {doc.UserSign} )");
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
			string query = " delete OCRG where GroupCode = " + code;
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

	public DataTable Search(CardGroupsImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT GroupCode,GroupName,GroupType,\r\n\t\t\t\t\t\t\t\t\tCASE \r\n\t\t\t\t\t\t\t\t\t\tWHEN Locked = 'N' then 'Locked'\r\n\t\t\t\t\t\t\t\t\t    WHEN Locked = 'Y' then 'Changeable'\r\n\t\t\t\t\t\t\t\t\tEND Locked,\r\n                                    UserSign\r\n\t\t\t\t\t\t\t   FROM  OCRG    \r\n                                Where 1=1";
			if (doc.Locked == "Y")
			{
				text += "AND Locked = 'Y'";
			}
			else if (doc.Locked == "N")
			{
				text += "AND Locked = 'N'";
			}
			if (!string.IsNullOrEmpty(doc.GroupName))
			{
				text = text + "AND GroupName like N'%" + doc.GroupName + "%' ";
			}
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Card groups Search - Doc ", ex.Message);
		}
		return result;
	}

	public bool Check(int code)
	{
		try
		{
			string query = "SELECT GroupCode \r\n\t\t\t\t\t\t\t        FROM  OCRG\r\n                                    WHERE GroupCode = " + code;
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
