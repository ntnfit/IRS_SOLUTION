using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class BoMJobRepository : BaseService<BillOfMasterials_Impl_OITT>, IBoMJob
{
	public BoMJobRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string SaveDoc(DataSet ds)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			_ = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				string query = "APZ_BoMJob_SaveDoc";
				list.Add(new SqlParameter("@dt", ds.Tables[0]));
				int num = ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray());
				if (num > 0)
				{
					string text = "";
					string father = ds.Tables[1].Rows[0]["Father"].ToString();
					text = SaveRawMater(ds.Tables[1]);
					if (!string.IsNullOrEmpty(text))
					{
						dbTransaction.Rollback();
						return text + " ==> Save Raw Mater";
					}
					text = SaveBoMJob(ds.Tables[2], father);
					if (!string.IsNullOrEmpty(text))
					{
						dbTransaction.Rollback();
						return text + " ==> Save BoM Job";
					}
					text = SaveBoMJobItem(ds.Tables[3], father);
					if (!string.IsNullOrEmpty(text))
					{
						dbTransaction.Rollback();
						return text + " ==> Save BoM Job Item";
					}
				}
				if (num > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return "ERROR";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Doc  FAILED ", ex.Message);
				dbTransaction.Rollback();
				return ex.Message;
			}
		}
	}

	private string SaveRawMater(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_SaveRawMater";
			list.Add(new SqlParameter("@dt", dt));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Raw Mater FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private string SaveBoMJob(DataTable dt, string father)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text = "";
			if (dt.Rows.Count > 0)
			{
				text = "APZ_BoMJob_SaveBoMJob";
				list.Add(new SqlParameter("@dt", dt));
				list.Add(new SqlParameter("@father", father));
			}
			else
			{
				text = "APZ_BoMJob_DeleteJobs";
				list.Add(new SqlParameter("@father", father));
			}
			if (ExecuteNonQuery(text, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save BoM Job FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private string SaveBoMJobItem(DataTable dt, string father)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text = "";
			if (dt.Rows.Count > 0)
			{
				text = "APZ_BoMJob_SaveBoMJobItem";
				list.Add(new SqlParameter("@dt", dt));
				list.Add(new SqlParameter("@father", father));
			}
			else
			{
				text = "APZ_BoMJob_DeleteJobItems";
				list.Add(new SqlParameter("@father", father));
			}
			if (ExecuteNonQuery(text, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save BoM Job Item FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetDocs()
	{
		try
		{
			string query = "APZ_BoMJob_GetDocs";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDocs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDoc(string Code)
	{
		try
		{
			string query = "APZ_BoMJob_GetDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", Code));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRawMater(string father)
	{
		try
		{
			string query = "APZ_BoMJob_GetRawMater";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@father", father));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRawMater  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetJob(string father)
	{
		try
		{
			string query = "APZ_BoMJob_GetJob";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@father", father));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetJob  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetJobItems(string father)
	{
		try
		{
			string query = "APZ_BoMJob_GetJobItems";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Father", father));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetJobItems  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public int GetNewJobNum()
	{
		try
		{
			string query = "APZ_BoMJob_GetNewJobNum";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNewJobNum  FAILED ", ex.Message);
			return -1;
		}
	}

	public DataTable GetResourceCFL()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_BoMJob_GetResourceCFL";
			return ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable IsRawMaterExists(List<string> lst)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_IsRawMaterExists";
			list.Add(new SqlParameter("@father", lst[0]));
			list.Add(new SqlParameter("@childNum", Function.ParseInt(lst[1])));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "IsRawMaterExists FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable IsJobExists(List<string> lst)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_IsJobExists";
			list.Add(new SqlParameter("@father", lst[0]));
			list.Add(new SqlParameter("@childNum", Function.ParseInt(lst[1])));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "IsJobExists FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable IsBoMExists(string itemCode)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_IsBoMExists";
			list.Add(new SqlParameter("@code", itemCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "IsJobExists FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable IsJobItemExists(List<string> lst)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_IsJobItemExists";
			list.Add(new SqlParameter("@father", lst[0]));
			list.Add(new SqlParameter("@fatherNum", Function.ParseInt(lst[1])));
			list.Add(new SqlParameter("@lineNum", Function.ParseInt(lst[2])));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "IsJobItemExists FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchDoc(DataTable dtSearch)
	{
		try
		{
			DataRow dataRow = dtSearch.Rows[0];
			string query = "APZ_BoMJob_SearchDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ProdNo", Function.ToString(dataRow["ProdNo"])));
			list.Add(new SqlParameter("@SOrderNo", Function.ParseInt(dataRow["SOrderNo"])));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchDoc FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string DeleteDocs(string ids)
	{
		try
		{
			string[] array = ids.Split(',');
			int num = 0;
			string[] array2 = array;
			foreach (string id in array2)
			{
				if (string.IsNullOrEmpty(DeleteDoc(id)))
				{
					num++;
				}
			}
			if (num > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDocs FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(string id)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_DeleteDocs";
			list.Add(new SqlParameter("@id", id));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDoc FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteRawMaters(string fatherAndchildNums)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_DeleteRawMaters";
			string[] array = fatherAndchildNums.Split(',');
			string text = "";
			for (int i = 1; i < array.Length; i++)
			{
				text = text + array[i] + ",";
			}
			text = text.Remove(text.Length - 1);
			list.Add(new SqlParameter("@father", array[0]));
			list.Add(new SqlParameter("@childNum", text));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteRawMaters FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteJobs(string fatherAndchildNums)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_DeleteJobs";
			string[] array = fatherAndchildNums.Split(',');
			string text = "";
			for (int i = 1; i < array.Length; i++)
			{
				text = text + array[i] + ",";
			}
			text = text.Remove(text.Length - 1);
			list.Add(new SqlParameter("@father", array[0]));
			list.Add(new SqlParameter("@childNum", text));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteJobs FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteJobItems(string fatherFNumAndlineNums)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_BoMJob_DeleteJobItems";
			string[] array = fatherFNumAndlineNums.Split(',');
			string text = "";
			for (int i = 2; i < array.Length; i++)
			{
				text = text + array[i] + ",";
			}
			text = text.Remove(text.Length - 1);
			list.Add(new SqlParameter("@father", array[0]));
			list.Add(new SqlParameter("@fatherNum", array[1]));
			list.Add(new SqlParameter("@lineNum", text));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteJobItems FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
