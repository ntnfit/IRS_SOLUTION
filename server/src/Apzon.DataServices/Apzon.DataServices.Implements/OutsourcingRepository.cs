using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class OutsourcingRepository : Repository<DataTable>, IOutsourcing, IDisposable
{
	private readonly IUnitOfWork _unitOfWork;

	public OutsourcingRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public string SaveDoc(List<DocumentsImpl> lst, string sessionId)
	{
		try
		{
			HttpResult httpResult = _unitOfWork.Documents.AddListDocument(lst, sessionId);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				string[] array = (httpResult.Content as string).Split(',');
				if (array != null && array.Length == 2)
				{
					DateTime? docDate = Function.ParseDateTime(lst[0].DocDate);
					string comments = lst[0].Comments;
					string acctCode = lst[0].Lines[0].AcctCode;
					int issueID = Function.ParseInt(array[0]);
					int receiptID = Function.ParseInt(array[1]);
					return SaveUDT(docDate, comments, acctCode, issueID, receiptID);
				}
				if (lst.Count == 1)
				{
					return "";
				}
				return "No return keys";
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string SaveUDT(DateTime? docDate, string comments, string clearAct, int issueID, int receiptID)
	{
		try
		{
			string query = "APZ_Outsourcing_SaveUDT";
			SqlParameter[] array = new SqlParameter[5];
			if (docDate.HasValue)
			{
				_ = docDate.Value;
				array[0] = new SqlParameter("@docDate", docDate.Value);
			}
			else
			{
				array[0] = new SqlParameter("@docDate", DBNull.Value);
			}
			array[1] = new SqlParameter("@comments", comments);
			array[2] = new SqlParameter("@clearAct", clearAct);
			array[3] = new SqlParameter("@issueID", issueID);
			array[4] = new SqlParameter("@receiptID", receiptID);
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, array) < 1)
			{
				return "[Outsourcing DocNum UDT] - No row inserted.";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public void Dispose()
	{
		Dispose();
	}
}
