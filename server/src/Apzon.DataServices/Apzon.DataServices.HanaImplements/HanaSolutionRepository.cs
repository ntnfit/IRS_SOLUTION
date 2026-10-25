using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaSolutionRepository : SolutionRepository
{
	public HanaSolutionRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult AddNew(DataSet doc, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			string xmlObject = Mapping(doc, EnumCommandType.Add);
			HttpResult httpResult = DataService.AddObject(objType, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				if (absEntry > 0)
				{
					DeleteFile(absEntry);
				}
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"Add new Solution - Id - {num}");
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			if (absEntry > 0)
			{
				DeleteFile(absEntry);
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override string UpdateData(DataSet doc, string sessionId, ApzObjectTypes objType, int absEntry)
	{
		try
		{
			string xmlObject = Mapping(doc, EnumCommandType.Update);
			HttpResult httpResult = DataService.UpdateObject(objType, sessionId, xmlObject, Function.ParseInt(doc.Tables[0].Rows[0]["SltCode"]));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", $"update document - {objType} - DocEntry - {num}");
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string DeleteDoc(int sltCode, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.RemoveObject(ApzObjectTypes.oKnowledgeBaseSolutions, sessionId, sltCode);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			Logging.Write("WATCH", $"Remove a Solution - {sltCode}");
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public override string Mapping(DataSet doc, EnumCommandType commandType)
	{
		DataTable dataTable = doc.Tables[0];
		string text = "{\r\n";
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ItemCode"])) || commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("ItemCode", Function.ToString(dataTable.Rows[0]["ItemCode"]));
			}
			if (Function.ParseInt(dataTable.Rows[0]["StatusNum"]) != 0 || commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("Status", Function.ParseInt(dataTable.Rows[0]["StatusNum"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Subject"])) || commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("Solution", Function.ToString(dataTable.Rows[0]["Subject"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Symptom"])) || commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("Symptom", Function.ToString(dataTable.Rows[0]["Symptom"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Cause"])) || commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("Cause", Function.ToString(dataTable.Rows[0]["Cause"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Descriptio"])) || commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("Description", Function.ToString(dataTable.Rows[0]["Descriptio"]));
			}
			text = ((Function.ParseInt(dataTable.Rows[0]["AtcEntry"]) == 0 || !doc.Tables[1].IsNotNull()) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", Function.ToString(dataTable.Rows[0]["AtcEntry"]))));
			IEnumerable<string> enumerable = from t in (from DataColumn c in dataTable.Columns
					select c.ColumnName).ToList()
				where t.StartsWith("U_")
				select t;
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				List<UserFieldsImpl> list = new List<UserFieldsImpl>();
				for (int num = 0; num < enumerable.ToList().Count; num++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (dataTable.Columns[enumerable.ToList()[num]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(dataTable.Rows[0][enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataTable.Columns[enumerable.ToList()[num]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDouble(dataTable.Rows[0][enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(dataTable.Rows[0][enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = enumerable.ToList()[num];
					list.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list);
			}
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
