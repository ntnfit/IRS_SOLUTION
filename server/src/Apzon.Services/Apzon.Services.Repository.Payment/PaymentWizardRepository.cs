using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Payment;

namespace Apzon.Services.Repository.Payment;

public class PaymentWizardRepository : BaseService<DataSet>, IPaymentWizard
{
	public PaymentWizardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetData(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dtSearch.IsNotNull())
			{
				DateTime? dateTime = Function.ParseDateTime(dtSearch.Rows[0]["fromdate"]);
				DateTime? dateTime2 = Function.ParseDateTime(dtSearch.Rows[0]["todate"]);
				string value = Function.ToString(dtSearch.Rows[0]["bpCode"]);
				int num = Function.ParseInt(dtSearch.Rows[0]["invoiceNo"]);
				string value2 = Function.ToString(dtSearch.Rows[0]["userName"]);
				int num2 = Function.ParseInt(dtSearch.Rows[0]["branch"]);
				if (!string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["fromDate"])))
				{
					list.Add(new SqlParameter("@fromdate", dateTime));
				}
				else
				{
					list.Add(new SqlParameter("@fromdate", null));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["toDate"])))
				{
					list.Add(new SqlParameter("@toDate", dateTime2));
				}
				else
				{
					list.Add(new SqlParameter("@toDate", null));
				}
				list.Add(new SqlParameter("@bpCode", value));
				list.Add(new SqlParameter("@invoiceNo", num));
				list.Add(new SqlParameter("@userName", value2));
				list.Add(new SqlParameter("@branch", num2));
				return ExecuteData("Apz_PaymentWizard_GetData", CommandType.StoredProcedure, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetPaymentInfo(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dtSearch.IsNotNull())
			{
				string value = Function.ToString(dtSearch.Rows[0]["userName"]);
				int num = Function.ParseInt(dtSearch.Rows[0]["branch"]);
				list.Add(new SqlParameter("@userName", value));
				list.Add(new SqlParameter("@branch", num));
				return ExecuteData("Apz_PaymentWizard_GetPaymentInfo", CommandType.StoredProcedure, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDetails(DataTable dtSearch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dtSearch.IsNotNull())
			{
				string value = Function.ToString(dtSearch.Rows[0]["userName"]);
				int num = Function.ParseInt(dtSearch.Rows[0]["branch"]);
				list.Add(new SqlParameter("@userName", value));
				list.Add(new SqlParameter("@branch", num));
				return ExecuteData("Apz_PaymentWizard_GetDetails", CommandType.StoredProcedure, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string Save(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oAdvancedInPayment, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
