using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaUsersRepository : UsersRepository
{
	public HanaUsersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService, IUnitOfWork dataUnitOfWork)
		: base(serviceUnitOfWork, dataService, dataUnitOfWork)
	{
	}

	protected override string Process(UsersImpl doc, string sessionId, EnumCommandType commandType)
	{
		try
		{
			string xmlObject = Mapping(doc, commandType, sessionId);
			HttpResult httpResult = ((commandType == EnumCommandType.Add) ? DataService.AddObject(ApzObjectTypes.oUsers, sessionId, xmlObject) : DataService.UpdateObject(ApzObjectTypes.oUsers, sessionId, xmlObject, doc.USERID));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				switch (commandType)
				{
				case EnumCommandType.Add:
					Logging.Write("ERROR", "Add new user error: " + httpResult.Message);
					break;
				case EnumCommandType.Update:
					Logging.Write("ERROR", "Update user error: " + httpResult.Message);
					break;
				}
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	protected override string Mapping(UsersImpl doc, EnumCommandType commandType, string sessionId)
	{
		string text = "{\r\n";
		if (commandType == EnumCommandType.Update)
		{
			text += Function.GetJsonString("InternalKey", doc.USERID);
		}
		if (commandType == EnumCommandType.Add || doc.IsChangePassword)
		{
			text += Function.GetJsonString("UserPassword", doc.PASSWORD);
		}
		text += Function.GetJsonString("UserCode", doc.USER_CODE);
		text += Function.GetJsonString("UserName", doc.U_NAME);
		text = ((!(Function.ToString(doc.SUPERUSER).ToLower() == "y")) ? (text + Function.GetJsonString("Superuser", "tNO")) : (text + Function.GetJsonString("Superuser", "tYES")));
		text += Function.GetJsonString("eMail", doc.E_Mail);
		text += Function.GetJsonString("MobilePhoneNumber", doc.PortNum);
		text += Function.GetJsonString("FaxNumber", doc.Fax);
		text = ((doc.Branch <= 0) ? (text + Function.GetJsonString("Branch", -2)) : (text + Function.GetJsonString("Branch", doc.Branch)));
		text = ((doc.Department <= 0) ? (text + Function.GetJsonString("Department", -2)) : (text + Function.GetJsonString("Department", doc.Department)));
		text = ((!(Function.ToString(doc.Locked).ToLower() == "y")) ? (text + Function.GetJsonString("Locked", "tNO")) : (text + Function.GetJsonString("Locked", "tYES")));
		text += Function.GetJsonString("Group", "ug_Regular");
		text += Function.GetJsonString("Defaults", doc.DfltsGroup);
		text += DataService.MappingUdf(doc.UserFields);
		if (doc.UserBranchAssignment != null)
		{
			text += "\"UserBranchAssignment\":[";
			for (int i = 0; i < doc.UserBranchAssignment.Count; i++)
			{
				text += "{";
				text += Function.GetJsonString("BPLID", doc.UserBranchAssignment[i].BPLId);
				text += "},";
			}
			text += "],";
		}
		return text + "}";
	}

	public override string DeleteDoc(int code, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				ObjectType = ApzObjectTypes.oUsers,
				Key = code,
				OutObjectKey = true
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}
}
