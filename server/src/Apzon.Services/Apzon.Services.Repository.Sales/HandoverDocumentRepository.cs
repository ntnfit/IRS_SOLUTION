using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Sales;

namespace Apzon.Services.Repository.Sales;

public class HandoverDocumentRepository : BaseService<DocumentsImpl>, IHandoverDocument
{
	public HandoverDocumentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet LoadDocuments(int id, string barcode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", id);
			return ExecuteData("Apz_HandoverDocument_LoadDocuments", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetById(int id)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", id);
			return ExecuteDataTable("select * from [@OHDC] where DocEntry = @DocEntry", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int GetPage(int current, int type, string username, int branch)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Apz_HandoverDocument_Navigation", CommandType.StoredProcedure, new SqlParameter("@Current", current), new SqlParameter("@Type", type), new SqlParameter("@Username", username), new SqlParameter("@Branch", branch));
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataSet GetDocumentDetail(string empCode, string barcode, string userName, int branch)
	{
		try
		{
			string query = "Apz_HandoverDocument_GetDocumentDetail";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Barcode", barcode));
			list.Add(new SqlParameter("@EmpCode", empCode));
			list.Add(new SqlParameter("@UserName", userName));
			list.Add(new SqlParameter("@Branch", branch));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet SearchDocuments(DocumentsImpl doc)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (doc.DocEntry > 0)
			{
				list.Add(new SqlParameter("@DocEntry", doc.DocEntry));
			}
			if (!string.IsNullOrEmpty(doc.CardCode))
			{
				list.Add(new SqlParameter("@Barcode", doc.CardCode));
			}
			if (!string.IsNullOrEmpty(doc.CardName))
			{
				list.Add(new SqlParameter("@EmpCode", doc.CardName));
			}
			if (!string.IsNullOrEmpty(doc.UserName))
			{
				list.Add(new SqlParameter("@UserName", doc.UserName));
			}
			if (doc.Branch > 0)
			{
				list.Add(new SqlParameter("@Branch", doc.Branch));
			}
			if (doc.CreateDate > new DateTime(1900, 1, 1))
			{
				list.Add(new SqlParameter("@CreateDate", doc.CreateDate));
			}
			return ExecuteData("Apz_HandoverDocument_SearchDocuments", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}
}
