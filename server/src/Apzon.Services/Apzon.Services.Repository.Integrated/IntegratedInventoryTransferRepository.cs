using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Integrated;

namespace Apzon.Services.Repository.Integrated;

public class IntegratedInventoryTransferRepository : BaseService<DataTable>, IIntegratedInventoryTransfer
{
	public IntegratedInventoryTransferRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet Search(DataTable doc)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_IntegratedInventoryTransfer_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@FromDate", Convert.ToDateTime(doc.Rows[0]["FromDate"])));
			list.Add(new SqlParameter("@ToDate", Convert.ToDateTime(doc.Rows[0]["ToDate"])));
			list.Add(new SqlParameter("@InvType", Function.ToString(doc.Rows[0]["InvType"])));
			list.Add(new SqlParameter("@IsImported", Function.ToString(doc.Rows[0]["IsImported"])));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet SearchLine(string id)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_IntegratedInventoryTransfer_SearchLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable SearchLineForSave(string id)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_IntegratedInventoryTransfer_SearchLineForSave";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string SaveADoc(DocumentsImpl doc)
	{
		return "";
	}

	public DataTable CheckExists(DataTable dt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_IntegratedInventoryTransfer_CheckExists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ID", Function.ToString(dt.Rows[0]["ID"])));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}
}
