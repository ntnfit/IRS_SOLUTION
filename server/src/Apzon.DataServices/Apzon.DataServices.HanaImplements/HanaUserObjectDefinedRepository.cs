using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaUserObjectDefinedRepository : UserObjectDefinedRepository
{
	public HanaUserObjectDefinedRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected virtual bool CheckUDOExists(string udoCode)
	{
		try
		{
			return ExecuteDataTable("select 1 from OUDO where \"Code\" = @code;", CommandType.Text, new SqlParameter("@code", Function.ToString(udoCode))).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	protected override string Mapping(DataSet ds, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			DataTable dataTable = ds.Tables[0];
			DataTable dataTable2 = ds.Tables[1];
			DataTable dataTable3 = ds.Tables[2];
			DataTable dataTable4 = ds.Tables[3];
			DataTable dataTable5 = ds.Tables[4];
			empty = "{";
			string value = Function.ToString(dataTable.Rows[0]["Code"]);
			string value2 = Function.ToString(dataTable.Rows[0]["LogTable"]);
			foreach (DataRow row in dataTable.Rows)
			{
				if (commandType == EnumCommandType.Add)
				{
					empty += Function.GetJsonString("Code", value);
					empty += Function.GetJsonString("TableName", Function.ToString(row["TableName"]));
					empty += Function.GetJsonString("LogTableName", value2);
					empty = ((Function.ParseInt(row["TYPE"]) != 1) ? (empty + Function.GetJsonString("ObjectType", "boud_Document")) : (empty + Function.GetJsonString("ObjectType", "boud_MasterData")));
					empty += Function.GetJsonString("MenuItem", Function.ToString(row["MenuItem"]).Equals("Y") ? "tYES" : "tNO");
					empty += Function.GetJsonString("FatherMenuID", Function.ParseInt(row["FatherMenu"]));
					empty += Function.GetJsonString("Position", Function.ParseInt(row["Position"]));
					empty += Function.GetJsonString("MenuUID", Function.ToString(row["MenuUid"]));
				}
				empty += Function.GetJsonString("Name", Function.ToString(row["Name"]));
				empty += Function.GetJsonString("ExtensionName", Function.ToString(row["ExtName"]));
				empty += Function.GetJsonString("MenuCaption", Function.ToString(row["MenuCapt"]));
				empty += Function.GetJsonString("CanCancel", Function.ToString(row["CanCancel"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("CanDelete", Function.ToString(row["CanDelete"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("CanLog", Function.ToString(row["CanLog"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("CanClose", Function.ToString(row["CanClose"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("CanCreateDefaultForm", Function.ToString(row["CanDefForm"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("CanFind", Function.ToString(row["CanFind"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("CanArchive", Function.ToString(row["CanArchive"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("EnableEnhancedForm", Function.ToString(row["CanNewForm"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("RebuildEnhancedForm", Function.ToString(row["IsRebuild"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("ManageSeries", Function.ToString(row["MngSeries"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("CanYearTransfer", Function.ToString(row["CanYrTrnsf"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("OverwriteDllfile", Function.ToString(row["OvrWrtDll"]).Equals("Y") ? "tYES" : "tNO");
				empty += Function.GetJsonString("FormSRF", Function.ToString(row["NewFormSrf"]));
			}
			empty += "UserObjectMD_ChildTables:[";
			foreach (DataRow row2 in dataTable2.Rows)
			{
				empty += "{";
				empty += Function.GetJsonString("SonNumber", Function.ParseInt(row2["SonNum"]));
				empty += Function.GetJsonString("TableName", Function.ToString(row2["TableName"]));
				empty += Function.GetJsonString("ObjectName", Function.ToString(row2["TableName"]));
				empty += "},";
			}
			empty += "],";
			empty += "UserObjectMD_FindColumns: [";
			foreach (DataRow row3 in dataTable3.Rows)
			{
				if (commandType == EnumCommandType.Update && !Function.ParseBool(row3["IsCheck"]))
				{
					string query = "delete from UDO2 where \"Code\" = @code and \"ColAlias\" = @colAlias;";
					ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@code", value), new SqlParameter("@colAlias", Function.ToString(row3["ColAlias"])));
				}
				else
				{
					empty += "{";
					empty += Function.GetJsonString("ColumnNumber", 0);
					empty += Function.GetJsonString("ColumnAlias", Function.ToString(row3["ColAlias"]));
					empty += Function.GetJsonString("ColumnDescription", Function.ToString(row3["ColumnDesc"]));
					empty += "},";
				}
			}
			empty += "],";
			empty += "UserObjectMD_FormColumns: [";
			foreach (DataRow row4 in dataTable4.Rows)
			{
				if (commandType == EnumCommandType.Update && !Function.ParseBool(row4["IsVisible"]))
				{
					string query2 = "delete from UDO3 where \"Code\" = @code and \"ColAlias\" = @colAlias;";
					ExecuteNonQuery(query2, CommandType.Text, new SqlParameter("@code", value), new SqlParameter("@colAlias", Function.ToString(row4["ColAlias"])));
					continue;
				}
				empty += "{";
				if (dataTable4.Columns.Contains("ColAlias"))
				{
					empty += Function.GetJsonString("FormColumnAlias", Function.ToString(row4["ColAlias"]));
				}
				empty += Function.GetJsonString("FormColumnDescription", Function.ToString(row4["ColDesc"]));
				empty += Function.GetJsonString("FormColumnNumber", 0);
				empty += Function.GetJsonString("Editable", Function.ParseBool(row4["IsEdit"]) ? "tYES" : "tNO");
				empty += "},";
			}
			empty += "],";
			empty += "UserObjectMD_EnhancedFormColumns: [";
			foreach (DataRow row5 in dataTable5.Rows)
			{
				empty += "{";
				empty += Function.GetJsonString("ColumnNumber", 0);
				empty += Function.GetJsonString("ChildNumber", Function.ParseInt(row5["SonNum"]));
				empty += Function.GetJsonString("ColumnAlias", Function.ToString(row5["ColAlias"]));
				empty += Function.GetJsonString("ColumnDescription", Function.ToString(row5["ColDesc"]));
				empty += Function.GetJsonString("ColumnIsUsed", Function.ParseBool(row5["ColIsUsed"]) ? "tYES" : "tNO");
				empty += Function.GetJsonString("Editable", Function.ParseBool(row5["ColEdit"]) ? "tYES" : "tNO");
				empty += "},";
			}
			empty += "]";
			return empty + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override string SaveData(DataSet ds, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string bodyObject = Mapping(ds, commandType, sessionId);
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (commandType == EnumCommandType.Add)
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.AddNew,
					BodyObject = bodyObject,
					ObjectType = ApzObjectTypes.oUserObjectsMD,
					OutObjectKey = true
				});
			}
			else
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Update,
					BodyObject = bodyObject,
					Key = Function.ToString(ds.Tables[0].Rows[0]["Code"]),
					ObjectType = ApzObjectTypes.oUserObjectsMD,
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override HttpResult Delete(string code, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				Key = code,
				ObjectType = ApzObjectTypes.oUserObjectsMD,
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
