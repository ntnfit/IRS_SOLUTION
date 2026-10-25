using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaUserFieldsRepository : UserFieldsRepository
{
	public HanaUserFieldsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string GetByParams(string sessionId, UserFieldsImpl field)
	{
		try
		{
			if (field == null)
			{
				return string.Empty;
			}
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@tableName", field.TableName),
				new SqlParameter("@fieldID", field.FieldID)
			};
			return ExecuteScarlar("select 1 from CUFD where \"TableID\" = tableName and \"FieldID\" = fieldID;", CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override HttpResult Process(string xml, string sessionId, string tableName, int fieldId, string command = "")
	{
		try
		{
			ApzObjectTypes objectType = ApzObjectTypes.oUserFields;
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = objectType,
				OutObjectKey = true,
				Key = new
				{
					TableName = tableName,
					FieldID = fieldId
				},
				Action = ((!(command == "Update")) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
				BodyObject = xml
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Save UDF error: " + httpResult.Message);
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			string text = Function.ToString(httpResult.Content);
			Logging.Write("WATCH", "Save UDF Success: " + string.Join(",", (command == "Add") ? text : $"(TableName='{tableName}',FieldID={fieldId})"));
			return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", (command == "Add") ? text : $"(TableName='{tableName}',FieldID={fieldId})"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override string Mapping(UserFieldsImpl field, string command)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			stringBuilder.Append(Function.GetJsonString("Name", field.Name));
			stringBuilder.Append(Function.GetJsonString("Size", field.Size));
			stringBuilder.Append(Function.GetJsonString("Description", field.Description));
			stringBuilder.Append(Function.GetJsonString("TableName", field.TableName));
			stringBuilder.Append(Function.GetJsonString("FieldID", field.FieldID));
			stringBuilder.Append(Function.GetJsonString("Mandatory", field.Mandatory));
			stringBuilder.Append(Function.GetJsonString("Type", field.Type.ToString()));
			stringBuilder.Append(Function.GetJsonString("SubType", field.SubType.ToString()));
			stringBuilder.Append(Function.GetJsonString("EditSize", field.EditSize));
			if (!string.IsNullOrEmpty(field.LinkedTable))
			{
				stringBuilder.Append(Function.GetJsonString("LinkedTable", field.LinkedTable));
			}
			if (!string.IsNullOrEmpty(field.DefaultValue))
			{
				stringBuilder.Append(Function.GetJsonString("DefaultValue", field.DefaultValue));
			}
			if (!string.IsNullOrEmpty(field.LinkedUDO))
			{
				stringBuilder.Append(Function.GetJsonString("LinkedUDO", field.LinkedUDO));
			}
			if (!string.IsNullOrEmpty(field.LinkedObject))
			{
				stringBuilder.Append(Function.GetJsonString("LinkedSystemObject", field.LinkedObject));
			}
			if (field.ValidValues != null && field.ValidValues.Count > 0)
			{
				stringBuilder.Append("\"ValidValuesMD\":[");
				foreach (ValidValuesMDImpl validValue in field.ValidValues)
				{
					stringBuilder.Append("{");
					if (field.SubType == BoFldSubTypes.st_Time)
					{
						stringBuilder.Append(Function.GetJsonString("Value", Function.ParseInt(validValue.Value.Replace(":", "")).ToString("0000")));
					}
					else
					{
						stringBuilder.Append(Function.GetJsonString("Value", validValue.Value));
					}
					stringBuilder.Append(Function.GetJsonString("Description", validValue.Description));
					stringBuilder.Append("},");
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append("]");
			}
			stringBuilder.Append("}");
			if (field.TableName.Equals("AITT") || field.TableName.Equals("ATT1"))
			{
				string text = (field.TableName.Equals("AITT") ? "APZ_OITT" : "APZ_ITT1");
				string text2;
				if (field.Type == BoFieldTypes.db_Numeric)
				{
					text2 = "int";
				}
				else if (field.Type == BoFieldTypes.db_Date)
				{
					text2 = "timestamp";
				}
				else if (field.Type != BoFieldTypes.db_Alpha)
				{
					text2 = ((field.Type == BoFieldTypes.db_Float) ? "numeric(19,6)" : "nvarchar(5000)");
				}
				else
				{
					text2 = $"nvarchar({field.EditSize})";
				}
				string sql = string.Format(" ALTER TABLE {0} {3} (U_{1} {2} null);", text, field.Name, text2, command.Equals("Add") ? "ADD" : "ALTER");
				stringBuilder.Append(_unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true));
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public override string RemoveUdf(string tableID, int fieldID, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oUserFields,
				OutObjectKey = true,
				Key = new
				{
					TableName = tableID,
					FieldID = fieldID
				},
				Action = Enums.HttpAction.Delete
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Remove UDF error: " + httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Remove UDF Success: " + Function.ToString(fieldID));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
