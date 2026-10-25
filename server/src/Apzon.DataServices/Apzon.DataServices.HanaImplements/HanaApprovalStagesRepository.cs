using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaApprovalStagesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService) : ApprovalStagesRepository(serviceUnitOfWork, unitOfWork, dataService)
{
	private ApzObjectTypes _objType = ApzObjectTypes.oApprovalStages;

	public override string Mapping(DataRow document, DataTable documentLine, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string text = "{";
			text += Function.GetJsonString("Code", Function.ToString(document["WstCode"]));
			text += Function.GetJsonString("Name", Function.ToString(document["Name"]));
			text += Function.GetJsonString("NoOfApproversRequired", Function.ParseInt(document["MaxReqr"]));
			text += Function.GetJsonString("Remarks", Function.ToString(document["Remarks"]));
			text += "\"ApprovalStageApprovers\":[";
			for (int i = 0; i < documentLine.Rows.Count - 1; i++)
			{
				text += "{";
				text += Function.GetJsonString("UserID", Function.ParseInt(documentLine.Rows[i]["UserID"]));
				text += "},";
			}
			text += "]";
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override HttpResult Save(Tuple<DataTable, DataTable, string> tuple, string sessionId)
	{
		try
		{
			EnumCommandType enumCommandType = (tuple.Item3.Equals("A") ? EnumCommandType.Add : EnumCommandType.Update);
			string bodyObject = Mapping(tuple.Item1.Rows[0], tuple.Item2, enumCommandType, sessionId);
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (enumCommandType == EnumCommandType.Add)
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.AddNew,
					BodyObject = bodyObject,
					ObjectType = _objType,
					OutObjectKey = true
				});
			}
			else
			{
				string obj = Function.ToString(tuple.Item1.Rows[0]["WstCode"]);
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Update,
					ObjectType = _objType,
					OutObjectKey = true,
					BodyObject = bodyObject,
					Key = Function.ParseInt(obj)
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult Delete(int stageCode, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				ObjectType = _objType,
				Key = stageCode,
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
