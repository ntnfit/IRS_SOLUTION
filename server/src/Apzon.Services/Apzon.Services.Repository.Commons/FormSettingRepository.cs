using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class FormSettingRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IFormSetting
{
	protected string Temp_OFST = "#APZ_OFST";

	protected string Temp_FST1 = "#APZ_FST1";

	protected string Temp_FST2 = "#APZ_FST2";

	protected const string tempTableSetting = "#tempSetting";

	protected Enums.DatabaseSystemType dbSysType;

	protected string saveType = "";

	protected string RestoreUserCode = "";

	protected string RestoreFormId = "";

	protected string RestoreItemId = "";

	public DataTable GetFormSetting(string formId, string itemId, string user, string lngCode)
	{
		try
		{
			string query = "apz_formsetting_get";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@FormId", formId ?? ""), new SqlParameter("@Itemid", itemId ?? ""), new SqlParameter("@UserSign", user ?? "-"), new SqlParameter("@lngCode", lngCode ?? "en-US"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public virtual string SaveData(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return "Data save is empty";
			}
			string value = Function.ToString(dt.Rows[0]["UserSign"]);
			string value2 = Function.ToString(dt.Rows[0]["FormId"]);
			string value3 = Function.ToString(dt.Rows[0]["ItemId"]);
			if (string.IsNullOrEmpty(value))
			{
				return "User save setting can not empty";
			}
			if (string.IsNullOrEmpty(value2))
			{
				return "Form save setting can not empty";
			}
			if (string.IsNullOrEmpty(value3))
			{
				return "Item save setting can not empty";
			}
			dt = StandardizedTable(dt, "APZ_FORMSETTING");
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			saveType = "B";
			return Process(ApzObjectTypes.oFormSetting, -1, Enums.TransactionType.U, dataSet).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult ProcessSave(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || ds.Tables.Count < 1 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
			}
			else
			{
				DataTable dataTable = ds.Tables[0];
				CreateTempTable(dataTable, "#tempSetting");
				BulkCopy(dataTable, "#tempSetting");
				string text = Function.ToString(dataTable.Rows[0]["UserSign"]);
				string text2 = Function.ToString(dataTable.Rows[0]["FormId"]);
				string text3 = Function.ToString(dataTable.Rows[0]["ItemId"]);
				int num = ExecuteNonQuery("apz_formsetting_save", CommandType.StoredProcedure, new SqlParameter("@userSign", text), new SqlParameter("@formId", text2), new SqlParameter("@itemId", text3));
				Logging.Write("TRACE", "FormSetting_Save", text, text2, text3, num.ToString());
				if (num < 0)
				{
					return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
				}
				objId = string.Join(",", text2, text3, text);
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult ProcessSaveCopy(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || ds.Tables.Count < 1 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
			}
			else
			{
				DataTable dataTable = ds.Tables[0];
				ExecuteNonQuery("apz_formsetting_copy", CommandType.StoredProcedure, new SqlParameter("@ItemId", Function.ToString(dataTable.Rows[0]["ItemId"])), new SqlParameter("@FormId", Function.ToString(dataTable.Rows[0]["FormId"])), new SqlParameter("@UserName", Function.ToString(dataTable.Rows[0]["UserName"])), new SqlParameter("@CopyUser", Function.ToString(dataTable.Rows[0]["CopyUser"])), new SqlParameter("@CopyAll", Function.ToString(dataTable.Rows[0]["CopyAll"])), new SqlParameter("@UserUpdate", Function.ToString(dataTable.Rows[0]["UserUpdate"])));
				string text = Function.ToString(dataTable.Rows[0]["CopyUser"]);
				List<string> list = new List<string>();
				string[] array = text.Split(',');
				foreach (string text2 in array)
				{
					list.Add(string.Join(",", Function.ToString(dataTable.Rows[0]["FormId"]), Function.ToString(dataTable.Rows[0]["ItemId"]), text2));
				}
				objId = string.Join(";", list.ToArray());
				Logging.Write("WATCH", "COPY FORMSETTING", Function.ToString(dataTable.Rows[0]["FormId"]), Function.ToString(dataTable.Rows[0]["ItemId"]), Function.ToString(dataTable.Rows[0]["UserName"]), Function.ToString(dataTable.Rows[0]["CopyUser"]), Function.ToString(dataTable.Rows[0]["CopyAll"]), Function.ToString(dataTable.Rows[0]["UserUpdate"]));
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string CopyFormSetting(DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dt);
				saveType = "C";
				return Process(ApzObjectTypes.oFormSetting, -1, Enums.TransactionType.U, dataSet).Message;
			}
			return "Selected data is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string CopyFormSettingToItem(int settingId)
	{
		try
		{
			if (settingId > 0)
			{
				saveType = "C2";
				return Process(ApzObjectTypes.oFormSetting, -1, Enums.TransactionType.U, null, Function.ToString(settingId)).Message;
			}
			return "Selected data is null";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult ProcessSaveCopyToItem(Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			ExecuteNonQuery("apz_formsetting_copyToItem", CommandType.StoredProcedure, new SqlParameter("@settingId", Function.ParseInt(objId)));
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet GetStructure(int id, string formId, string itemId, string userName, int branch, string type)
	{
		try
		{
			string query = "Apz_FormSetting_GetStructure";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@formId", formId));
			list.Add(new SqlParameter("@itemId", itemId));
			list.Add(new SqlParameter("@type", type));
			list.Add(new SqlParameter("@username", userName));
			list.Add(new SqlParameter("@branch", branch));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetListFormSetting(string formId, string itemId, string user, int id)
	{
		try
		{
			string query = "Apz_FormSetting_GetListFormSetting";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@formId", formId));
			list.Add(new SqlParameter("@itemId", itemId));
			list.Add(new SqlParameter("@username", user));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetValidData(string formId, string itemId, string user, int id)
	{
		try
		{
			string query = "Apz_FormSetting_GetValidData";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@id", id));
			list.Add(new SqlParameter("@formId", formId));
			list.Add(new SqlParameter("@itemId", itemId));
			list.Add(new SqlParameter("@username", user));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult Save(DataSet ds, string objId = "", Enums.TransactionType transactionType = Enums.TransactionType.A, Enums.DatabaseSystemType databaseSystemType = Enums.DatabaseSystemType.SQL)
	{
		try
		{
			Logging.Write("WATCH", "Start Save " + ApzObjectTypes.oReportFormSetting);
			dbSysType = databaseSystemType;
			return Process(ApzObjectTypes.oReportFormSetting, -1, transactionType, ds, objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			if (saveType == "B")
			{
				return ProcessSave(dsSave, transtype, ref objId, ref objNum);
			}
			if (saveType == "C")
			{
				return ProcessSaveCopy(dsSave, transtype, ref objId, ref objNum);
			}
			if (saveType == "C2")
			{
				return ProcessSaveCopyToItem(transtype, ref objId, ref objNum);
			}
			if (saveType.Equals("R"))
			{
				ExecuteNonQuery("delete from APZ_FORMSETTING where \"UserSign\" <> '-' and \"UserSign\" = @useSign and \"FormId\" = @formId and \"ItemId\" = @itemId;", CommandType.Text, new SqlParameter("@useSign", RestoreUserCode), new SqlParameter("@formId", RestoreFormId), new SqlParameter("@itemId", RestoreItemId));
				return new HttpResult();
			}
			if (saveType.Equals("RA"))
			{
				ExecuteNonQuery("delete from APZ_FORMSETTING where \"UserSign\" <> '-' and \"FormId\" = @formId and \"ItemId\" = @itemId;", CommandType.Text, new SqlParameter("@formId", RestoreFormId), new SqlParameter("@itemId", RestoreItemId));
				return new HttpResult();
			}
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return Delete(Function.ParseInt(objId));
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || ds.Tables.Count < 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
				return httpResult;
			}
			if (dbSysType == Enums.DatabaseSystemType.HANA)
			{
				for (int i = 0; i < ds.Tables["FST1"].Columns.Count; i++)
				{
					if (ds.Tables["FST1"].Columns[i].DataType == typeof(bool))
					{
						ds.Tables["FST1"].ChangeColumnDataType(ds.Tables["FST1"].Columns[i].ColumnName, typeof(byte));
					}
				}
			}
			CreateTempTable(ds.Tables["OFST"], Temp_OFST);
			BulkCopy(ds.Tables["OFST"], Temp_OFST);
			CreateTempTable(ds.Tables["FST1"], Temp_FST1);
			BulkCopy(ds.Tables["FST1"], Temp_FST1);
			if (ds.Tables["FST2"].IsNotNull())
			{
				CreateTempTable(ds.Tables["FST2"], Temp_FST2);
				BulkCopy(ds.Tables["FST2"], Temp_FST2);
			}
			DataTable dataTable = ExecuteDataTable("Apz_FormSetting_SaveProcess", CommandType.StoredProcedure);
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					objNum = Function.ToString(dataTable.Rows[0][1]);
					Logging.Write("WATCH", $"Save {ApzObjectTypes.oReportFormSetting} Success", objNum);
					return new HttpResult();
				}
				string text = Function.ToString(dataTable.Rows[0][1]);
				Logging.Write("WATCH", $"Save {ApzObjectTypes.oReportFormSetting} Error", text);
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{text}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet GetUserForCopy(string userName, int idSetting)
	{
		try
		{
			string query = "Apz_FormSetting_GetUserForCopy";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@idSetting", idSetting));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult CopyReportFormSetting(string listUser, int idSetting)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "Apz_FormSetting_CopyReportFormSetting";
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@listUser", listUser));
				list.Add(new SqlParameter("@idSetting", idSetting));
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
				if (dataTable.IsNotNull())
				{
					dbTransaction.Rollback();
					return new HttpResult(Enums.MessageCode.Error, Function.ToString(dataTable.Rows[0][0]));
				}
				dbTransaction.Commit();
				return new HttpResult(Enums.MessageCode.Success);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Delete(int id)
	{
		SqlParameter sqlParameter = new SqlParameter("@id", id);
		if (ExecuteNonQuery("Apz_FormSetting_Delete", CommandType.StoredProcedure, sqlParameter) >= 0)
		{
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Delete error!");
	}

	public DataTable GetDataLanguage(string lngCode)
	{
		try
		{
			return ExecuteDataTable("Select a.\"TrlLng\" \"Code\", b.\"LngName\" \"Name\" from APZ_LNG1 a inner join APZ_OLNG b ON a.\"TrlLng\" = b.\"LngCode\"\r\n                                        where b.\"Status\" = 'A' and a.\"LngCode\" = @lngCode;", CommandType.Text, new SqlParameter("@lngCode", Function.ToString(lngCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetDataSettingLanguage(string currLng)
	{
		try
		{
			return ExecuteDataTable("select a.\"LngCode\", case when b.\"LngCode\" is null then a.\"LngName\" else b.\"TrlName\" end \"LngName\" from APZ_OLNG a \r\n                                        left join (select * from APZ_LNG1 where \"LngCode\" = @lngCode) b ON a.\"LngCode\" = b.\"TrlLng\"  where a.\"Status\" = 'A';", CommandType.Text, new SqlParameter("@lngCode", currLng));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult RestoreDefault(string userCode, string formId, string itemId)
	{
		RestoreUserCode = userCode;
		RestoreFormId = formId;
		RestoreItemId = itemId;
		saveType = "R";
		return Process(ApzObjectTypes.oFormSetting, -1, Enums.TransactionType.U, null);
	}

	public HttpResult RestoreDefaultAllUser(string formId, string itemId)
	{
		saveType = "RA";
		RestoreFormId = formId;
		RestoreItemId = itemId;
		return Process(ApzObjectTypes.oFormSetting, -1, Enums.TransactionType.U, null);
	}

	public DataTable GetMessageDataCenter(string lngCode)
	{
		try
		{
			return ExecuteDataTable("select * from APZ_OMDC where \"MsgLng\" = @lngCode;", CommandType.Text, new SqlParameter("@lngCode", lngCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}
}
