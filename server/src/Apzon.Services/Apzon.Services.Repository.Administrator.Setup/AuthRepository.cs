using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class AuthRepository : BaseService<DataTable>, IAuths
{
	public AuthRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetAllAuth(string user, string lngCode = "")
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@userName", user)
			};
			if (!string.IsNullOrWhiteSpace(lngCode))
			{
				list.Add(new SqlParameter("@lngCode", lngCode));
			}
			new DataTable();
			string query = "Apz_Auth_GetAllAuth";
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable DMSGetAllAuth(string user)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", user));
			new DataTable();
			string query = "APZ_D_Auth_GetAllAuth";
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveDocs(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oIRSAuth, -1, Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveDMSAuthorization(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oDMSAuthorization, -1, Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			string text = string.Empty;
			switch (objtype)
			{
			case ApzObjectTypes.oDataOwnerShipAuthorization:
			{
				CreateTempTable(dt, "#HEM5");
				BulkCopy(dt, "#HEM5");
				DataTable dataTable = ExecuteDataTable("APZ_Process_DataOwnerShipAuthorization", CommandType.StoredProcedure);
				if (dataTable.IsNotNull())
				{
					text = Function.ToString(dataTable.Rows[0][0]);
				}
				break;
			}
			case ApzObjectTypes.oDMSAuthorization:
				text = SaveDataAuthDMSProcess(dt);
				break;
			default:
				text = SaveDataProcess(dt);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				return new HttpResult(Enums.MessageCode.Error, text);
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string SaveData(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string result = SaveDataProcess(dt);
				dbTransaction.Commit();
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string SaveDataProcess(DataTable dt)
	{
		string value = Function.ToString(dt.Rows[0]["USER_CODE"]);
		ExecuteNonQuery(GetSaveDataProcessQueryDelete(), CommandType.Text, new SqlParameter("@userCode", value));
		DataTable tableStruct = GetTableStruct("OPRM");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dt, "#APZ_AUTH");
		BulkCopy(dt, "#APZ_AUTH");
		ExecuteNonQuery(GetSaveDataProcessQueryUpdateData(), CommandType.Text, new SqlParameter("@userCode", value));
		return "";
	}

	private string SaveDataAuthDMSProcess(DataTable dt)
	{
		string value = Function.ToString(dt.Rows[0]["USER_CODE"]);
		DataTable tableStruct = GetTableStruct("APZ_D_OPRM");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dt, "#APZ_D_AUTH");
		BulkCopy(dt, "#APZ_D_AUTH");
		DataTable dataTable = ExecuteDataTable("APZ_D_Auth_Update", CommandType.StoredProcedure, new SqlParameter("@userCode", value));
		string text = "";
		if (dataTable.IsNotNull())
		{
			return Function.ToString(dataTable.Rows[0]["Message"]);
		}
		return "Insert APZ_D_OPRM fail";
	}

	protected virtual string GetSaveDataProcessQueryDelete()
	{
		return "Delete OPRM where USER_CODE = @userCode";
	}

	protected virtual string GetSaveDataProcessQueryUpdateData()
	{
		return "INSERT INTO OPRM ([USER_CODE],[menucode],[authoriza],[isDefault])\r\n                                SELECT [USER_CODE],[menucode],[authoriza],[isDefault]\r\n                                FROM #APZ_AUTH";
	}

	public DataTable GetFormAuth(string user, string menu)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@menu", Function.ToString(menu)));
			list.Add(new SqlParameter("@user", Function.ToString(user)));
			new DataTable();
			string query = "SELECT t1.menucode FROM apz_omni T0 LEFT JOIN oprm T1 ON T0.Code=t1.menucode and t1.USER_CODE=@user AND T1.authoriza IN ('F') and T1.meucode=@menu e";
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Form Auth  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllAuthForm(string user, string lngCode, string superUser)
	{
		try
		{
			return ExecuteDataTable("Apz_Auth_GetAllAuthForm", CommandType.StoredProcedure, new SqlParameter("@user", Function.ToString(user)), new SqlParameter("@lngCode", lngCode), new SqlParameter("@superUser", superUser));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAllAuthForm FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string CopyAuth(List<string> lstUser)
	{
		string result = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string value = lstUser[0];
				lstUser.RemoveAt(0);
				string value2 = string.Join(",", lstUser.ToArray());
				string query = "apz_authorization_copy";
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@source", value), new SqlParameter("@target", value2)) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					result = "ERROR";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CopyAuth  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public string CopyAuthDMS(List<string> lstUser)
	{
		string result = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string value = lstUser[0];
				lstUser.RemoveAt(0);
				string value2 = string.Join(",", lstUser.ToArray());
				string query = "APZ_D_Authorization_Copy";
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@source", value), new SqlParameter("@target", value2)) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					result = "ERROR";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CopyAuth  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetUserForCopy(string user)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@user", Function.ToString(user)));
			return ExecuteDataTable(GetUserForCopyQuery(), CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetUserForCopy  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetUserForCopyQuery()
	{
		return "select  USER_CODE,U_NAME ,isnull(b.Name,'') as Department from OUSR a\r\n                    left join OUDP b on b.Code = a.Department  where GROUPS<> 99 and user_code<>@user";
	}

	public string InsertFormsToAuthTbl(DataTable dt)
	{
		string result = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt, "#APZ_AUTH");
				BulkCopy(dt, "#APZ_AUTH");
				DataTable dataTable = ExecuteDataTable("Apz_Auth_InsertFormsToAuthTbl", CommandType.StoredProcedure);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					result = Function.ToString(dataTable.Rows[0][0]);
				}
				else
				{
					dbTransaction.Commit();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "InsertFormsToAuthTbl  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable GetOwnerShipAuthorization(int empId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@EmpId", empId);
			return ExecuteDataTable("apz_auth_ownership", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveOwnerShip(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oDataOwnerShipAuthorization, -1, Enums.TransactionType.U, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string CheckExistsMemu()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetCheckExistsMemuQuery(), CommandType.Text);
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "N";
	}

	protected virtual string GetCheckExistsMemuQuery()
	{
		return "IF EXISTS (SELECT 1 FROM dbo.APZ_OMNI)\r\n                                        BEGIN\r\n\t                                        SELECT 'Y'\r\n                                        END\r\n                                        ELSE\r\n                                        BEGIN\r\n\t                                        SELECT 'N'\r\n                                        END\t";
	}

	public HttpResult CopyOwner(List<int> lstEmp)
	{
		string message = "";
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				int num = lstEmp[0];
				lstEmp.RemoveAt(0);
				string value = string.Join(",", lstEmp.ToArray());
				string query = "apz_ownership_copy";
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@source", num), new SqlParameter("@target", value)) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					message = "ERROR";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CopyAuth owner  FAILED ", ex.Message);
			message = ex.Message;
		}
		return new HttpResult
		{
			Message = message
		};
	}
}
