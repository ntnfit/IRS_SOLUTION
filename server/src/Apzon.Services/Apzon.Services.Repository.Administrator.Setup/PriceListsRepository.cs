using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class PriceListsRepository(IDatabaseService databaseService) : BaseService<PriceListImpl>(databaseService), IPriceLists, IService<PriceListImpl>
{
	private DataTable dt = new DataTable();

	public List<PriceListImpl> Cfl()
	{
		List<PriceListImpl> list = new List<PriceListImpl>();
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"ListNum\", \"ListName\" FROM OPLN WHERE \"ValidFor\" = 'Y';", CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new PriceListImpl
				{
					PriceListNo = Function.ParseInt(dataTable.Rows[i]["ListNum"]),
					PriceListName = Function.ToString(dataTable.Rows[i]["ListName"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public string Insert(PriceListImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(PriceListImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(PriceListImpl entity)
	{
		throw new NotImplementedException();
	}

	public PriceListImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetGroupNumPriceList()
	{
		try
		{
			string query = "select * from  OPLN order by \"ListName\";";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable ShowPriceList()
	{
		try
		{
			string query = ShowPriceListQuery();
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string ShowPriceListQuery()
	{
		return "select ListNum as PriceListNo ,ListName as PriceListName,UserSign from  OPLN";
	}

	public DataTable Search(PriceListImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT \"ListNum\" AS \"PriceListNo\", \"ListName\" AS \"PriceListName\", \"UserSign\" FROM OPLN \r\n                                Where 1=1 ";
			if (!string.IsNullOrEmpty(doc.PriceListName))
			{
				text = text + "AND \"ListName\" like '%" + doc.PriceListName + "%' ";
			}
			result = ExecuteData(text + ";", CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data PriceList Search - Doc ", ex.Message);
		}
		return result;
	}

	public int GetMaxCode()
	{
		try
		{
			string maxCodeQuery = GetMaxCodeQuery();
			DataTable dataTable = ExecuteDataTable(maxCodeQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["PriceListNo"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	protected virtual string GetMaxCodeQuery()
	{
		return "SELECT ISNULL( MAX(ListNum ),0) as PriceListNo\r\n                            FROM OPLN ";
	}

	public bool Check(int idCode)
	{
		try
		{
			string query = "SELECT   A.\"ListNum\" \r\n                            FROM OPLN A \r\n                            WHERE A.\"ListNum\" = '" + idCode + "';";
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

	public string SaveDoc(PriceListImpl doc)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			string empty = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				bool flag = Check(doc.PriceListNo);
				string empty2 = string.Empty;
				if (!flag)
				{
					empty2 = "INSERT INTO OPLN  (\"ListNum\" , \"ListName\",\"UserSign\")\r\n                                                     VALUES \r\n                                        ( ";
					empty2 += " @listNum ";
					list.Add(new SqlParameter("@listNum", doc.PriceListNo));
					empty2 += ", @listName ";
					list.Add(new SqlParameter("@listName", doc.PriceListName));
					empty2 += ", @userSign ";
					list.Add(new SqlParameter("@userSign", doc.UserSign));
					empty2 += " )";
				}
				else
				{
					list = new List<SqlParameter>();
					empty2 = "UPDATE OPLN  \r\n                            SET ";
					empty2 += "\"ListName\"= @listName ";
					list.Add(new SqlParameter("@listName", doc.PriceListName));
					empty2 += ",\"UserSign\"= @userSign ";
					list.Add(new SqlParameter("@userSign", doc.UserSign));
					empty2 += " WHERE \"ListNum\" =@listNum";
					list.Add(new SqlParameter("@listNum", doc.PriceListNo));
				}
				if (ExecuteNonQuery(empty2 + ";", CommandType.Text, list.ToArray()) > 0)
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

	public string DeleteDoc(int idCode)
	{
		try
		{
			string query = " delete from OPLN where \"ListNum\" = '" + idCode + "';";
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

	public DataTable GetPriceList(string itmCode)
	{
		try
		{
			string text = GetPriceListQuery();
			if (!string.IsNullOrEmpty(itmCode))
			{
				text = text + "AND a.\"ItemCode\" = N'" + itmCode + "' ";
			}
			dt = ExecuteData(text + ";", CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return dt;
	}

	protected virtual string GetPriceListQuery()
	{
		return "SELECT a.\"ItemCode\", b.\"ListNum\" AS \"PriceListNo\", ISNULL(b.\"ListName\", '') AS \"PriceListName\", ISNULL(a.\"Price\", 0) AS \"Price\", ISNULL(b.\"IsGrossPrc\", '') AS \"IsGrossPrc\", ISNULL(a.\"Currency\", '') AS \"Currency\" FROM ITM1 a LEFT OUTER JOIN OPLN b ON a.\"PriceList\" = b.\"ListNum\"\r\n                 where  1= 1  ";
	}
}
