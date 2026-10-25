using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Integrated;

namespace Apzon.Services.Repository.Integrated;

public class IntegratedWarehouseRepository : BaseService<DataTable>, IIntegratedWarehouse
{
	public IntegratedWarehouseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet Search(DataTable doc)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_Integrated_WareHouse_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@FromDate", Convert.ToDateTime(doc.Rows[0]["FromDate"])));
			list.Add(new SqlParameter("@ToDate", Convert.ToDateTime(doc.Rows[0]["ToDate"])));
			list.Add(new SqlParameter("@WhsName", Function.ToString(doc.Rows[0]["WhsName"])));
			list.Add(new SqlParameter("@IsImported", Function.ToString(doc.Rows[0]["IsImported"])));
			result = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable CheckExists(DataTable dt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_Integrated_WareHouse_CheckExists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ObjectType", Function.ParseInt(dt.Rows[0]["ObjectType"])));
			list.Add(new SqlParameter("@IsIns", Function.ToString(dt.Rows[0]["IsIns"])));
			list.Add(new SqlParameter("@IsReceiptFromProduct", Function.ToString(dt.Rows[0]["IsReceiptFromProduct"])));
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
