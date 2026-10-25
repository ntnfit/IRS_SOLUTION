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
using Apzon.Services.Interfaces.HumanResources.TimeSheeting;

namespace Apzon.Services.Repository.HumanResources.TimeSheeting;

public class TimeSheetingRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), ITimeSheeting
{
	private string processType = "";

	private string Temp_WorkType = "#APZ_OHWT";

	private string Temp_WorkCode = "#APZ_OHWC";

	private string Temp_OvertimeType = "#APZ_OHOT";

	private string Temp_OvertimeInformation = "#dtAPZ_TSL3";

	private string Temp_BioDataHeader = "#APZ_OTBD";

	private string Temp_BioDataLine = "#APZ_TBD1";

	private string Temp_InOutHeader = "#APZ_OIOD";

	private string Temp_InOutLine = "#APZ_IOD1";

	private string Temp_InOutOT = "#APZ_IOD2";

	private string TempTableTab = "#dtAPZ_OTSL";

	private string TempBiometricUserData = "#dtUserData";

	private string TempBioMapping = "#APZ_OTEM";

	private string TempConfig = "#APZ_OBMC";

	private string Temp_ImportTimeSheeting = "#dtAPZ_TSL1";

	private string Temp_ImportTimeSheetingOT = "#dtAPZ_TSH1";

	private string Temp_ImportTimeSheetingTemplate = "#dtAPZ_OTSI";

	public DataTable LoadTimeSheeting(int branch, string dept, int empID, string empName, string status, string period, string workcode, int baseEntry)
	{
		try
		{
			string query = "APZ_TimeSheeting_Load";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Branch", branch));
			list.Add(new SqlParameter("@Dept", dept ?? ""));
			list.Add(new SqlParameter("@Emp", Function.ParseInt(empID)));
			list.Add(new SqlParameter("@EmpName", empName ?? ""));
			list.Add(new SqlParameter("@SStatus", status ?? ""));
			list.Add(new SqlParameter("@Period", period ?? ""));
			list.Add(new SqlParameter("@BaseEntry", baseEntry));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadTimeSheetingSetting(string userSign)
	{
		try
		{
			string query = "SELECT * FROM APZ_TSLS WHERE \"UserSign\"=@UserSign;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserSign", userSign));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet LoadTimeSheetingCopyFrom(int baseEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@BaseEntry", baseEntry));
			DataSet dataSet = ExecuteData("APZ_TimeSheeting_LoadCopyFrom", CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables[0].Rows.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable ExportTimeSheeting(int branch, string dept, int empID, string empName, string status, string period)
	{
		try
		{
			string query = "APZ_TimeSheeting_Export";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Branch", branch));
			list.Add(new SqlParameter("@Dept", dept ?? ""));
			list.Add(new SqlParameter("@Emp", Function.ParseInt(empID)));
			list.Add(new SqlParameter("@EmpName", empName ?? ""));
			list.Add(new SqlParameter("@SStatus", status ?? ""));
			list.Add(new SqlParameter("@Period", period ?? ""));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string CancelTimeSheeting(string period)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Period", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Period"] = period;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			processType = "SaveTimeSheeting";
			return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.D, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult SaveTimeSheeting(DataTable dt)
	{
		processType = "SaveTimeSheeting";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public string SaveImportTimeSheeting(DataSet dt)
	{
		processType = "SaveImportTimeSheeting";
		return Process(ApzObjectTypes.oImportTimeSheeting, -1, Enums.TransactionType.A, dt).Message;
	}

	public string SaveImportTimeSheetingTemplate(DataTable dt)
	{
		processType = "SaveImportTimeSheetingTemplate";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oImportTimeSheetingTemplate, -1, Enums.TransactionType.A, dataSet).Message;
	}

	public string SaveWorkType(DataTable dt)
	{
		processType = "SaveWorkType";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oWorkTypes, -1, Enums.TransactionType.A, dataSet).Message;
	}

	public string SaveWorkCode(DataTable dt)
	{
		processType = "SaveWorkCode";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Function.ToString(Process(ApzObjectTypes.oWorkCode, -1, Enums.TransactionType.A, dataSet).Content);
	}

	public HttpResult SaveOvertimeType(DataTable dt)
	{
		processType = "SaveOvertimeType";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oOvertimeType, -1, Enums.TransactionType.A, dataSet);
	}

	protected virtual string DeleteWorkCodeQuery()
	{
		return "Delete from APZ_OHWC where Code = @code and Branch = @Branch and not exists(select Top 1  a.EmpId from APZ_TSL1 a join OHEM b on a.EmpId = b.empID where b.branch = @Branch and a.WorkCode = @Code)";
	}

	protected virtual string DeleteOvertimeTypeQuery()
	{
		return "Delete from APZ_OHOT where Code = @code and Branch = @Branch and not exists (select Top 1  a.EmpId from APZ_TSH1 a join OHEM b on a.EmpId = b.empID where b.branch = @Branch and a.OvertimeType = @Code)";
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		objId = "";
		objNum = "";
		string text = "";
		string text2 = "";
		HttpResult ret = new HttpResult();
		try
		{
			switch (processType)
			{
			case "SaveTimeSheeting":
			{
				if (transtype == Enums.TransactionType.D)
				{
					ProcessDelete(document, ref ret, ref objId, ref objNum);
					if (!string.IsNullOrWhiteSpace(ret.Message))
					{
						return new HttpResult(Enums.MessageCode.Error, ret.Message);
					}
					break;
				}
				CreateTempTable(document.Tables[0], TempTableTab);
				BulkCopy(document.Tables[0], TempTableTab);
				text = "APZ_TimeSheeting_Save";
				DataTable dataTable4 = ExecuteDataTable(text, CommandType.StoredProcedure);
				if (dataTable4.IsNotNull() && dataTable4.Columns.Contains("error_code"))
				{
					return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable4.Rows[0][0])}-{Function.ToString(dataTable4.Rows[0][1])}");
				}
				if (dataTable4.HasColumn() && dataTable4.IsNotNull())
				{
					string period = Function.ToString(dataTable4.Rows[0]["Period"]);
					string text4 = Function.ToString(dataTable4.Rows[0]["LineNum"]);
					string text5 = Function.ToString(dataTable4.Rows[0]["EmpList"]);
					string text6 = Function.ToString(dataTable4.Rows[0]["RowDeletedOTSL"]);
					string text7 = Function.ToString(dataTable4.Rows[0]["RowDeletedTSL1"]);
					string text8 = Function.ToString(dataTable4.Rows[0]["RowInsertedOTSL"]);
					string text9 = Function.ToString(dataTable4.Rows[0]["RowInsertedTSL1"]);
					text2 = period + "+" + text4 + "+" + text5 + "+" + text6 + "+" + text7 + "+" + text8 + "+" + text9;
					IEnumerable<string> values = from e in (from t in document.Tables[0].AsEnumerable()
							select t.Field<int>("EmpId")).Distinct()
						select string.Join(",", period, e);
					objId = string.Join(";", values);
					objNum = period;
				}
				break;
			}
			case "SaveImportTimeSheeting":
			{
				processType = "SaveImportTimeSheeting";
				CreateTempTable(document.Tables[0], Temp_ImportTimeSheeting);
				BulkCopy(document.Tables[0], Temp_ImportTimeSheeting);
				CreateTempTable(document.Tables[1], Temp_ImportTimeSheetingOT);
				BulkCopy(document.Tables[1], Temp_ImportTimeSheetingOT);
				text = "APZ_ImportTimeSheeting_Save";
				object obj3 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj3);
				objNum = objId;
				break;
			}
			case "SaveImportTimeSheetingTemplate":
			{
				CreateTempTable(document.Tables[0], Temp_ImportTimeSheetingTemplate);
				BulkCopy(document.Tables[0], Temp_ImportTimeSheetingTemplate);
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@UserSign", Function.ToString(document.Tables[0].Rows[0]["UserSign"])));
				list.Add(new SqlParameter("@TableName", Function.ToString(document.Tables[0].Rows[0]["TableName"])));
				object obj = ExecuteScalar("APZ_ImportTimeSheeting_SaveTemplate", CommandType.StoredProcedure, list.ToArray());
				objId = Function.ToString(obj);
				objNum = objId;
				break;
			}
			case "SaveWorkType":
			{
				CreateTempTable(document.Tables[0], Temp_WorkType);
				BulkCopy(document.Tables[0], Temp_WorkType);
				text = "APZ_SaveWorkType";
				DataTable dataTable2 = ExecuteDataTable(text, CommandType.StoredProcedure);
				if (Function.ParseInt(dataTable2.Rows[0][0]) == 1)
				{
					return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable2.Rows[0][1]));
				}
				objId = Function.ToString(dataTable2.Rows[0][1]);
				objNum = objId;
				break;
			}
			case "SaveWorkCode":
				if (transtype == Enums.TransactionType.D)
				{
					text = DeleteWorkCodeQuery();
					string value = Function.ToString(document.Tables[0].Rows[0]["Code"]);
					int num = Function.ParseInt(document.Tables[0].Rows[0]["Branch"]);
					List<SqlParameter> list2 = new List<SqlParameter>
					{
						new SqlParameter("@code", value),
						new SqlParameter("@Branch", num)
					};
					if (ExecuteNonQuery(text, CommandType.Text, list2.ToArray()) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "This Work code has been used!");
					}
				}
				else
				{
					CreateTempTable(document.Tables[0], Temp_WorkCode);
					BulkCopy(document.Tables[0], Temp_WorkCode);
					text = "APZ_SaveWorkCode";
					object obj2 = ExecuteScalar(text, CommandType.StoredProcedure);
					objId = Function.ToString(obj2);
					objNum = objId;
				}
				break;
			case "SaveOvertimeType":
				if (transtype == Enums.TransactionType.D)
				{
					text = DeleteOvertimeTypeQuery();
					string value2 = Function.ToString(document.Tables[0].Rows[0]["Code"]);
					int num2 = Function.ParseInt(document.Tables[0].Rows[0]["Branch"]);
					List<SqlParameter> list3 = new List<SqlParameter>
					{
						new SqlParameter("@code", value2),
						new SqlParameter("@Branch", num2)
					};
					if (ExecuteNonQuery(text, CommandType.Text, list3.ToArray()) <= 0)
					{
						return new HttpResult(Enums.MessageCode.Error, "This word code has been used!");
					}
				}
				else
				{
					CreateTempTable(document.Tables[0], Temp_OvertimeType);
					BulkCopy(document.Tables[0], Temp_OvertimeType);
					text = "APZ_SaveOvertimeType";
					object obj5 = ExecuteScalar(text, CommandType.StoredProcedure);
					if (Function.ParseInt(obj5) == 1)
					{
						return new HttpResult(Enums.MessageCode.Error, "Code already exists in the table!");
					}
					objId = Function.ToString(obj5);
					objNum = objId;
				}
				break;
			case "SaveOvertimeInformation":
			{
				CreateTempTable(document.Tables[0], Temp_OvertimeInformation);
				BulkCopy(document.Tables[0], Temp_OvertimeInformation);
				text = "APZ_TimeSheeting_SaveOvertimeInformation";
				object obj4 = ExecuteScalar(text, CommandType.StoredProcedure);
				objId = Function.ToString(obj4);
				objNum = objId;
				break;
			}
			case "SaveBiometricData":
			{
				if (transtype == Enums.TransactionType.D)
				{
					ProcessDelete(document, ref ret, ref objId, ref objNum);
					if (!string.IsNullOrWhiteSpace(ret.Message))
					{
						return new HttpResult(Enums.MessageCode.Error, ret.Message);
					}
					break;
				}
				CreateTempTable(document.Tables[0], Temp_BioDataHeader);
				BulkCopy(document.Tables[0], Temp_BioDataHeader);
				CreateTempTable(document.Tables[1], Temp_BioDataLine);
				BulkCopy(document.Tables[1], Temp_BioDataLine);
				text = "APZ_Biometric_Save";
				string text12 = "";
				DataTable dataTable6 = ExecuteDataTable(text, CommandType.StoredProcedure);
				if (dataTable6.HasColumn() && dataTable6.IsNotNull())
				{
					text12 = Function.ToString(dataTable6.Rows[0][0]);
				}
				objId = Function.ToString(text12);
				objNum = objId;
				break;
			}
			case "SaveTimeSheetInOut":
			{
				if (transtype == Enums.TransactionType.D)
				{
					ProcessDelete(document, ref ret, ref objId, ref objNum);
					if (!string.IsNullOrWhiteSpace(ret.Message))
					{
						return new HttpResult(Enums.MessageCode.Error, ret.Message);
					}
					break;
				}
				CreateTempTable(document.Tables[0], Temp_InOutHeader);
				BulkCopy(document.Tables[0], Temp_InOutHeader);
				CreateTempTable(document.Tables[1], Temp_InOutLine);
				BulkCopy(document.Tables[1], Temp_InOutLine);
				CreateTempTable(document.Tables[2], Temp_InOutOT);
				BulkCopy(document.Tables[2], Temp_InOutOT);
				text = "APZ_TimeSheetInOut_Save";
				DataTable dataTable3 = ExecuteDataTable(text, CommandType.StoredProcedure);
				if (dataTable3.IsNotNull() && string.IsNullOrEmpty(Function.ToString(dataTable3.Rows[0][1])))
				{
					objId = Function.ToString(dataTable3.Rows[0][0]);
					objNum = objId;
					break;
				}
				return new HttpResult(Enums.MessageCode.Error, "Cannot access to database");
			}
			case "SaveBiometricMapping":
			{
				CreateTempTable(document.Tables[0], TempBioMapping);
				BulkCopy(document.Tables[0], TempBioMapping);
				text = "APZ_BiometricMapping_Save";
				DataTable dataTable5 = ExecuteDataTable(text, CommandType.StoredProcedure);
				if (dataTable5.IsNotNull())
				{
					string text10 = Function.ToString(dataTable5.Rows[0][1]);
					if (!string.IsNullOrEmpty(text10))
					{
						return new HttpResult(Enums.MessageCode.Error, text10);
					}
					string text11 = "";
					objId = Function.ToString(text11);
					objNum = objId;
					break;
				}
				return new HttpResult(Enums.MessageCode.Error, "Cannot access to database");
			}
			case "SaveBiometricConfig":
			{
				if (transtype == Enums.TransactionType.D)
				{
					ProcessDelete(document, ref ret, ref objId, ref objNum);
					if (!string.IsNullOrWhiteSpace(ret.Message))
					{
						return new HttpResult(Enums.MessageCode.Error, ret.Message);
					}
					break;
				}
				CreateTempTable(document.Tables[0], TempConfig);
				BulkCopy(document.Tables[0], TempConfig);
				text = "APZ_BiometricConfig_Save";
				DataTable dataTable = ExecuteDataTable(text, CommandType.StoredProcedure);
				if (dataTable.HasColumn() && dataTable.IsNotNull())
				{
					return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][0]));
				}
				string text3 = "";
				objId = Function.ToString(text3);
				objNum = objId;
				break;
			}
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text2);
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string SaveTimeSheetingSetting(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@Branch", Function.ToString(dt.Rows[0]["Branch"])));
				list.Add(new SqlParameter("@Department", Function.ToString(dt.Rows[0]["Department"])));
				list.Add(new SqlParameter("@Status", Function.ToString(dt.Rows[0]["Status"])));
				list.Add(new SqlParameter("@Period", Function.ToString(dt.Rows[0]["Period"])));
				list.Add(new SqlParameter("@UserSign", Function.ToString(dt.Rows[0]["UserSign"])));
				if (ExecuteNonQuery(GetSaveTimeSheetingSettingQuery(), CommandType.Text, list.ToArray()) <= 0)
				{
					return "Error!";
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected virtual string GetSaveTimeSheetingSettingQuery()
	{
		return "UPDATE APZ_TSLS SET Branch=@Branch, Department=@Department, Status=@Status, Period=@Period, UserSign=@UserSign\r\n                            IF (@@ROWCOUNT = 0 ) \r\n                                BEGIN  \r\n                                    INSERT INTO APZ_TSLS (Branch, Department,Status, Period, UserSign)  \r\n                                    VALUES (@Branch, @Department,@Status,@Period,@UserSign)  \r\n                                END  ";
	}

	public DataTable LoadImportTimeSheetingTemplate(string tableName, string userSign)
	{
		try
		{
			string query = "SELECT * FROM APZ_OTSI WHERE \"TableName\"=@TableName AND \"UserSign\"=@UserSign;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserSign", userSign));
			list.Add(new SqlParameter("@TableName", tableName));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadOvertimeLeave(string period, int branch)
	{
		try
		{
			string query = "select a.* from APZ_TSL3 a INNER JOIN OHEM b ON a.\"EmpId\"=b.\"empID\"  where \"Period\" =@Period and b.\"branch\"=@Branch;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Period", period));
			list.Add(new SqlParameter("@Branch", branch));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			string query2 = "select TOP 0 * from APZ_TSL3";
			return ExecuteDataTable(query2, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadOvertimeLeave  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadOvertimeNew(string period, int branch)
	{
		try
		{
			string query = "APZ_TimeSheeting_LoadOvertimeNew";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Period", period));
			list.Add(new SqlParameter("@Branch", branch));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			string query2 = "select TOP 0 Period, EmpId, WorkTotal from APZ_OTSH";
			return ExecuteDataTable(query2, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadOvertimeLeave  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveOvertimeInformation(DataTable dt)
	{
		processType = "SaveOvertimeInformation";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oTimeSheeting, -1, Enums.TransactionType.A, dataSet);
	}

	public DataTable GetWorkType()
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OHWT;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadWorkType  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string RemoveWorkType(int Id)
	{
		string empty = string.Empty;
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				if (Id > 0)
				{
					SqlParameter sqlParameter = new SqlParameter("@Id", Id);
					if (ExecuteNonQuery("delete from APZ_OHWT where \"Id\"=@Id and \"Id\" NOT IN(select \"WorkType\" from APZ_OHWC);", CommandType.Text, sqlParameter) > 0)
					{
						dbTransaction.Commit();
						return "";
					}
					dbTransaction.Rollback();
					return "Cannot remove name because it has been used in Workcode";
				}
				dbTransaction.Rollback();
				return "You cannot remove record default in system";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetWorkCode(int type)
	{
		try
		{
			string text = "Select * From APZ_OHWC ";
			if (type == 2)
			{
				text += "Where \"Status\" = 'A' ";
			}
			text += "Order by \"Code\";";
			return ExecuteDataTable(text, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadWorkCode  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string RemoveWorkCode(string id, int branch)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Branch", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = id;
			dataRow["Branch"] = branch;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			processType = "SaveWorkCode";
			return Process(ApzObjectTypes.oWorkCode, -1, Enums.TransactionType.D, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string CheckWorkCode(string id, int branch)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using (DatabaseService.BeginTransaction())
				{
					List<SqlParameter> list = new List<SqlParameter>
					{
						new SqlParameter("@code", id),
						new SqlParameter("@Branch", branch)
					};
					if (Function.ParseInt(ExecuteDataTable("select isnull((select Top 1  a.EmpId from APZ_TSL1 a join OHEM b on a.EmpId = b.empID where b.branch = @Branch and a.WorkCode = @Code),0);", CommandType.Text, list.ToArray()).Rows[0][0]) == 0)
					{
						return "";
					}
					return Function.ToString("This work code has been used!");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetOvertimeType()
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OHOT;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadOvertimeType  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string RemoveOvertimeType(string code, int branch)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Branch", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = code;
			dataRow["Branch"] = branch;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			processType = "SaveOvertimeType";
			return Process(ApzObjectTypes.oOvertimeType, -1, Enums.TransactionType.D, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string CheckOvertimeType(string code)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using (DatabaseService.BeginTransaction())
				{
					SqlParameter sqlParameter = new SqlParameter("@code", code);
					if (Function.ParseInt(ExecuteDataTable("select isnull((Select Top 1 EmpId from APZ_TSH1 Where OvertimeType = @Code),0)", CommandType.Text, sqlParameter).Rows[0][0]) == 0)
					{
						return "";
					}
					return Function.ToString("This Overtime Type has been used!");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Delete FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public HttpResult SaveBiometricData(DataSet ds)
	{
		processType = "SaveBiometricData";
		return Process(ApzObjectTypes.oBiometricData, -1, Enums.TransactionType.A, ds);
	}

	public HttpResult SaveTimeSheetInOut(DataSet ds)
	{
		processType = "SaveTimeSheetInOut";
		return Process(ApzObjectTypes.oTimeSheetInOut, -1, Enums.TransactionType.A, ds);
	}

	public HttpResult SaveBiometricMapping(DataTable dt)
	{
		processType = "SaveBiometricMapping";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oBiometricMapping, -1, Enums.TransactionType.A, dataSet);
	}

	public HttpResult SaveBiometricConfig(DataTable dt)
	{
		processType = "SaveBiometricConfig";
		DataSet dataSet = new DataSet();
		dataSet.Tables.Add(dt);
		return Process(ApzObjectTypes.oBiometricConfig, -1, Enums.TransactionType.A, dataSet);
	}

	public DataTable LoadBiometricMapping(DataTable dt)
	{
		try
		{
			DataTable result;
			using (DatabaseService.OpenConnection())
			{
				processType = "LoadBiometricMapping";
				CreateTempTable(dt, TempBiometricUserData);
				BulkCopy(dt, TempBiometricUserData);
				string query = "APZ_Biometric_Mapping";
				result = ExecuteDataTable(query, CommandType.StoredProcedure);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadTimeSheetInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode)
	{
		try
		{
			string query = "APZ_TimeSheetInOut_Load";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Branch", branch));
			list.Add(new SqlParameter("@Dept", dept ?? ""));
			list.Add(new SqlParameter("@Emp", Function.ParseInt(empID)));
			list.Add(new SqlParameter("@EmpName", empName ?? ""));
			list.Add(new SqlParameter("@SStatus", status ?? ""));
			list.Add(new SqlParameter("@Period", period ?? ""));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet LoadInOutByBaseDoc(int absEntry)
	{
		try
		{
			string query = "APZ_InOutBaseDoc_Load";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AbsEntry", absEntry));
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable LoadTimeSheetingFromInOut(int branch, string dept, int empID, string empName, string status, string period, string workcode)
	{
		try
		{
			string query = "APZ_TimeSheeting_LoadFromInOut";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Branch", branch));
			list.Add(new SqlParameter("@Dept", dept ?? ""));
			list.Add(new SqlParameter("@Emp", Function.ParseInt(empID)));
			list.Add(new SqlParameter("@EmpName", empName ?? ""));
			list.Add(new SqlParameter("@SStatus", status ?? ""));
			list.Add(new SqlParameter("@Period", period ?? ""));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadClockReport(int branch, string dept, int empID, string empName, string status, string period)
	{
		try
		{
			string query = "APZ_TimeSheeting_LoadClockReport";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Branch", branch));
			list.Add(new SqlParameter("@Dept", dept ?? ""));
			list.Add(new SqlParameter("@Emp", Function.ParseInt(empID)));
			list.Add(new SqlParameter("@EmpName", empName ?? ""));
			list.Add(new SqlParameter("@SStatus", status ?? ""));
			list.Add(new SqlParameter("@Period", period ?? ""));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadClockReportByEntry(int absEntry)
	{
		try
		{
			string query = "APZ_Biometric_LoadByEntry";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AbsEntry", absEntry));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadImportList()
	{
		try
		{
			string query = "Select a.*\r\n\t\t\t\t\t\t\t\tFrom APZ_OTBD a\r\n\t\t\t\t\t\t\t\tOrder by a.\"AbsEntry\" DESC;";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadInOutList()
	{
		try
		{
			string query = "select * from APZ_OIOD order by \"AbsEntry\" DESC;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet LoadInOutByAbsEntry(int absEntry)
	{
		try
		{
			string query = "APZ_TimeSheeting_LoadInOutByAbsEntry";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@AbsEntry", absEntry));
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetAllBiometricConfig()
	{
		try
		{
			string query = "select * from APZ_OBMC;";
			new List<SqlParameter>();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string RemoveConfig(int absEntry)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AbsEntry", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["AbsEntry"] = absEntry;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			processType = "SaveBiometricConfig";
			return Process(ApzObjectTypes.oBiometricConfig, -1, Enums.TransactionType.D, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string RemoveBiometricData(int absEntry)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AbsEntry", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["AbsEntry"] = absEntry;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			processType = "SaveBiometricData";
			return Process(ApzObjectTypes.oBiometricData, -1, Enums.TransactionType.D, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string RemoveTimeSheetInOut(int absEntry)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AbsEntry", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["AbsEntry"] = absEntry;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			processType = "SaveTimeSheetInOut";
			return Process(ApzObjectTypes.oTimeSheetInOut, -1, Enums.TransactionType.D, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void ProcessDelete(DataSet ds, ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			string text = "";
			switch (processType)
			{
			case "SaveBiometricConfig":
				text = "DELETE from APZ_OBMC where \"AbsEntry\" = @absEntry;";
				break;
			case "SaveBiometricData":
				text = "DELETE from APZ_OTBD where \"AbsEntry\" = @absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_TBD1 where \"AbsEntry\" = @absEntry;";
				break;
			case "SaveTimeSheetInOut":
				text = "\r\n\t\t\t\t\t\t\t\tUPDATE APZ_OTBD set \"status\" = 'O' where \"AbsEntry\" in (select \"BaseDoc\" from APZ_OIOD where \"AbsEntry\" = @absEntry);\r\n\t\t\t\t\t\t\t\tDELETE from APZ_OIOD where \"AbsEntry\" = @absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_IOD1 where \"AbsEntry\" = @absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_IOD2 where \"AbsEntry\" = @absEntry;\r\n\t\t\t\t\t\t\t\t";
				break;
			case "SaveTimeSheeting":
				text = "UPDATE APZ_OIOD set \"status\"= 'O' where \"Period\"=@absEntry;\r\n\t\t\t\t\t\t\t\tUPDATE APZ_OHWP set \"PeriodStatus\"  = 'U' where \"PeriodCode\"=@absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_OTSL where \"Period\" = @absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_TSL1 where \"Period\" = @absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_TSL3 where \"Period\" = @absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_OTSH where \"Period\" = @absEntry;\r\n\t\t\t\t\t\t\t\tDELETE from APZ_TSH1 where \"Period\" = @absEntry;\r\n\t\t\t\t\t\t";
				break;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				ret.Message = "processType not found!";
			}
			List<SqlParameter> list = new List<SqlParameter>();
			if (ds.Tables[0].Columns.Contains("AbsEntry"))
			{
				list.Add(new SqlParameter("@absEntry", Function.ToString(ds.Tables[0].Rows[0]["AbsEntry"])));
			}
			if (ds.Tables[0].Columns.Contains("Period"))
			{
				list.Add(new SqlParameter("@absEntry", Function.ToString(ds.Tables[0].Rows[0]["Period"])));
			}
			int num = ExecuteNonQuery(text, CommandType.Text, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
			ret.Message = "Delete error: " + ex.Message;
		}
	}

	public DataSetResult GetDataSetting()
	{
		try
		{
			string query = "select \"SchID\", \"SchName\" from APZ_OWSC;\r\n\r\n\t\t\t\t\t\t\t\tselect \"Code\", \"Name\" from APZ_OHWC Where \"Status\" = 'A';\r\n\r\n\t\t\t\t\t\t\t\tselect \"PeriodCode\", \"PeriodName\" from APZ_OHWP;\r\n\r\n\t\t\t\t\t\t\t\tselect \"ShiftID\", \"ShiftCode\" from APZ_OEWS;\r\n\t\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\tselect \"Code\", \"Name\" from OUDP;\r\n\t\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\tselect \"USERID\" , \"U_NAME\" from OUSR;";
			return new DataSetResult(ExecuteData(query, CommandType.Text, (SqlParameter[])null));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public DataTable GetOvertimeTypeByBaseDoc(int absEntry)
	{
		try
		{
			string query = "Select Distinct(x.\"OvertimeType\")\r\n\t\t\t\t\t\t\t\tFrom APZ_IOD2 x LEFT JOIN APZ_OTSH y on x.\"Period\" = y.\"Period\" And x.\"OvertimeType\" = y.\"OvertimeType\" And x.\"AbsEntry\" = y.\"BaseEntry\"\r\n\t\t\t\t\t\t\t\tWhere x.\"AbsEntry\" = @AbsEntry AND y.\"OvertimeType\" IS NULL;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@AbsEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet LoadWorkCodeForEmpByDate(int inTime, int outTime, string period, int empId, DateTime WorkDate)
	{
		try
		{
			return ExecuteData("APZ_TimeSheetInOut_LoadWorkCodeForEmpByDate", CommandType.StoredProcedure, new SqlParameter("@inTime", inTime), new SqlParameter("@outTime", outTime), new SqlParameter("@period", period), new SqlParameter("@empId", empId), new SqlParameter("@workDay", WorkDate));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}
}
