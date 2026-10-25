using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.HumanResources;

namespace Apzon.Services.Repository.HumanResources;

public class StatesRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IStates
{
	private string processType = "";

	private string Temp_NewRole = "#APZ_Roles";

	private string Temp_TeamSetup = "#APZ_Teams";

	private string Temp_EmpPos = "#APZ_Pos";

	private string Tem_TerReason = "#APZ_TerminationReasons";

	private string Tem_EmpStatus = "#APZ_EmpStatus";

	private string Tem_Department = "#APZ_Department";

	private string Tem_Branches = "#APZ_Branches";

	private string Tem_States = "#APZ_States";

	public DataTable GetNewRoles()
	{
		try
		{
			string newRolesQuery = GetNewRolesQuery();
			return ExecuteDataTable(newRolesQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetNewRolesQuery()
	{
		return "select typeID, name ,descriptio,locked  From OHTY";
	}

	public string RemoveRoles(int objId)
	{
		string result = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveRolesQuery();
				SqlParameter sqlParameter = new SqlParameter("@typeID", objId);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					result = "Cannot remove role because it has been used in transactions ";
					dbTransaction.Rollback();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDoc FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	protected virtual string RemoveRolesQuery()
	{
		return "DELETE  FROM OHTY WHERE typeID = @typeID and @typeID NOT IN( SELECT roleID FROM HEM6)";
	}

	public DataTable GetEmployeeStatus()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("APZ_EmployeeMaster_GetStatus", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetTermReason()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("APZ_EmployeeMaster_GetTermReason", CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetBranchs()
	{
		DataTable result = new DataTable();
		try
		{
			string branchsQuery = GetBranchsQuery();
			result = ExecuteDataTable(branchsQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetBranchsQuery()
	{
		return "SELECT Code, Name FROM OUBR";
	}

	public DataTable GetCountryCode()
	{
		try
		{
			string countryCodeQuery = GetCountryCodeQuery();
			return ExecuteDataTable(countryCodeQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetCountryCodeQuery()
	{
		return "SELECT Code, Name FROM OCRY";
	}

	public DataTable GetTeamSetup()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OHTM", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveTeamSetup(int objId)
	{
		string result = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveTeamSetupQuery();
				SqlParameter sqlParameter = new SqlParameter("@teamID", objId);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
				}
				else
				{
					result = "Cannot remove team because it has been used in transactions";
					dbTransaction.Rollback();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDoc FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	protected virtual string RemoveTeamSetupQuery()
	{
		return "DELETE  FROM OHTM WHERE teamID = @teamID and @teamID NOT IN( SELECT teamID FROM HTM1)";
	}

	public DataTable GetEmpPos()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OHPS", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveEmpPos(int id)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveEmpPosQuery();
				SqlParameter sqlParameter = new SqlParameter("@posID", id);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No record effected";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string RemoveEmpPosQuery()
	{
		return "DELETE  FROM OHPS WHERE posID = @PosID";
	}

	public DataTable GetEmpStatus()
	{
		try
		{
			return ExecuteDataTable("select * From OHST", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveEmpStatus(int id)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveEmpStatusQuery();
				SqlParameter sqlParameter = new SqlParameter("@statusID", id);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return Function.ToString("No record effected");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string RemoveEmpStatusQuery()
	{
		return "DELETE  FROM OHST WHERE statusID = @statusID";
	}

	public DataTable GetTerminationReasons()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OHTR", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveTerminationReasons(int id)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveTerminationReasonsQuery();
				SqlParameter sqlParameter = new SqlParameter("@reasonID", id);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return Function.ToString("No record effected");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string RemoveTerminationReasonsQuery()
	{
		return "DELETE  FROM OHTR WHERE reasonID = @reasonID";
	}

	public DataTable GetDepartment()
	{
		try
		{
			return ExecuteDataTable("SELECT * FROM OUDP;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string RemoveDepartment(int id)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveDepartmentQuery();
				SqlParameter sqlParameter = new SqlParameter("@code", id);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "No record effected";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string RemoveDepartmentQuery()
	{
		return "DELETE  FROM OUDP WHERE Code = @code";
	}

	public DataTable GetBranches()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("select * from OUBR", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string RemoveBranches(int id)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveBranchesQuery();
				SqlParameter sqlParameter = new SqlParameter("@code", id);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return Function.ToString("No record effected");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string RemoveBranchesQuery()
	{
		return "Delete from OUBR Where Code = @code";
	}

	public DataTable GetStates()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("select * from OCST;", CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string RemoveStates(string code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = RemoveStatesQuery();
				SqlParameter sqlParameter = new SqlParameter("@code", code);
				if (ExecuteNonQuery(query, CommandType.Text, sqlParameter) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return Function.ToString("Error");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string RemoveStatesQuery()
	{
		return "Delete from OCST where Code = @code";
	}

	public DataTable LoadStates(string countryCode)
	{
		try
		{
			string query = LoadStatesQuery();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@Country", countryCode)
			};
			return ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string LoadStatesQuery()
	{
		return "select * from OCST where Country = @Country";
	}

	public HttpResult SaveNewRoles(DataTable dt)
	{
		processType = "SaveNewRoles";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveTeamSetup(DataTable dt)
	{
		processType = "SaveTeamSetup";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveEmpPos(DataTable dt)
	{
		processType = "SaveEmpPos";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveTerminationReasons(DataTable dt)
	{
		processType = "SaveTerminationReasons";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveEmpStatus(DataTable dt)
	{
		processType = "SaveEmpStatus";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveDepartment(DataTable dt)
	{
		processType = "SaveDepartment";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveBranches(DataTable dt)
	{
		processType = "SaveBranches";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveStates(DataTable dt)
	{
		processType = "SaveStates";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		try
		{
			switch (processType)
			{
			case "SaveNewRoles":
			{
				CreateTempTable(document.Tables[0], Temp_NewRole);
				BulkCopy(document.Tables[0], Temp_NewRole);
				text = "APZ_EmployeeMaster_InsertNewRoles";
				object obj8 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj8);
				objNum = objId;
				break;
			}
			case "SaveTeamSetup":
			{
				CreateTempTable(document.Tables[0], Temp_TeamSetup);
				BulkCopy(document.Tables[0], Temp_TeamSetup);
				text = "AAPZ_EmployeeMaster_SaveTeamsSetup";
				object obj7 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj7);
				objNum = objId;
				break;
			}
			case "SaveEmpPos":
			{
				CreateTempTable(document.Tables[0], Temp_EmpPos);
				BulkCopy(document.Tables[0], Temp_EmpPos);
				text = "APZ_EmployeeMaster_SaveEmpPos";
				object obj6 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj6);
				objNum = objId;
				break;
			}
			case "SaveTerminationReasons":
			{
				CreateTempTable(document.Tables[0], Tem_TerReason);
				BulkCopy(document.Tables[0], Tem_TerReason);
				text = "APZ_EmployeeMaster_SaveTerminationReasons";
				object obj5 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj5);
				objNum = objId;
				break;
			}
			case "SaveEmpStatus":
			{
				CreateTempTable(document.Tables[0], Tem_EmpStatus);
				BulkCopy(document.Tables[0], Tem_EmpStatus);
				text = "APZ_EmployeeMaster_SaveEmpStatus";
				object obj4 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj4);
				objNum = objId;
				break;
			}
			case "SaveDepartment":
			{
				CreateTempTable(document.Tables[0], Tem_Department);
				BulkCopy(document.Tables[0], Tem_Department);
				text = "APZ_EmployeeMaster_SaveDepartment";
				object obj3 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj3);
				objNum = objId;
				break;
			}
			case "SaveBranches":
			{
				CreateTempTable(document.Tables[0], Tem_Branches);
				BulkCopy(document.Tables[0], Tem_Branches);
				text = "APZ_EmployeeMaster_SaveBranches";
				object obj2 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj2);
				objNum = objId;
				break;
			}
			case "SaveStates":
			{
				CreateTempTable(document.Tables[0], Tem_States);
				BulkCopy(document.Tables[0], Tem_States);
				text = "APZ_EmployeeMaster_SaveStates";
				object obj = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj);
				objNum = objId;
				break;
			}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
