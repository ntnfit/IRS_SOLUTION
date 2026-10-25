using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

public class DMSApprovedRepository : BaseService<DataTable>, IDMSApproved
{
	public DMSApprovedRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetList(string userName)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[1]
			{
				new SqlParameter
				{
					ParameterName = "@userName",
					Value = userName
				}
			};
			string query = "APZ_D_Approved_List";
			return ExecuteDataTable(query, CommandType.StoredProcedure, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListV2(string userName, string fromDate, string toDate)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[3]
			{
				new SqlParameter
				{
					ParameterName = "@userName",
					Value = userName
				},
				new SqlParameter
				{
					ParameterName = "@fromDate",
					Value = fromDate
				},
				new SqlParameter
				{
					ParameterName = "@toDate",
					Value = toDate
				}
			};
			string query = "APZ_D_Approved_ListV2";
			return ExecuteDataTable(query, CommandType.StoredProcedure, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public ResponseResult GetListItem(string objType, string userCode, string fromDate, string toDate, int pageIndex, string typeFilter, string searchText, int wddCode)
	{
		try
		{
			string query = "APZ_D_Approved_ListItem";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@objType", objType),
				new SqlParameter("@userCode", userCode),
				new SqlParameter("@fromDate", fromDate),
				new SqlParameter("@toDate", toDate),
				new SqlParameter("@pageIndex", pageIndex),
				new SqlParameter("@typeFilter", typeFilter),
				new SqlParameter("@searchText", searchText),
				new SqlParameter("@wddCode", wddCode)
			};
			DataTable content = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			return new ResponseResult
			{
				Content = content,
				Message = "",
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	public ResponseResult GetDetailItem(string userName, string status, string requestType, string wddCode)
	{
		try
		{
			string query = "APZ_D_Approval_GetDetailItem";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@userCode", userName),
				new SqlParameter("@status", status),
				new SqlParameter("@requestType", requestType),
				new SqlParameter("@wddEntry", wddCode)
			};
			DataTable content = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			return new ResponseResult
			{
				Content = content,
				Message = "",
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}

	public ResponseResult GetTemplateDetails(int wddCode, int wtmCode)
	{
		try
		{
			string query = "APZ_D_Approval_GetTemplateDetails";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@wddCode", wddCode),
				new SqlParameter("@wtmCode", wtmCode)
			};
			DataTable content = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			return new ResponseResult
			{
				Content = content,
				Message = "",
				MessageCode = "200"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "400"
			};
		}
	}
}
