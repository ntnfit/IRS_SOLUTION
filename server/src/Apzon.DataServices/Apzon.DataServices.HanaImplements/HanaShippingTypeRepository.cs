using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaShippingTypeRepository : ShippingTypeRepository
{
	public HanaShippingTypeRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string Delete(int trnspCode, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = ApzObjectTypes.oShippingTypes,
				OutObjectKey = true,
				Key = trnspCode,
				Action = Enums.HttpAction.Delete
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Delete Shipping type error: " + httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Delete Shipping type Success: " + Function.ToString(trnspCode));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult Save(DataTable dt, string sessionId)
	{
		try
		{
			ApzObjectTypes objectType = ApzObjectTypes.oShippingTypes;
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (dt != null && dt.Rows.Count > 0)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string empty = string.Empty;
					empty += Mapping(dt.Rows[i], (!Exists(Function.ToString(dt.Rows[i]["TrnspCode"]), sessionId)) ? EnumCommandType.Add : EnumCommandType.Update);
					list.Add(new BodyRequestObject
					{
						ObjectType = objectType,
						OutObjectKey = true,
						Key = Function.ParseInt(dt.Rows[i]["TrnspCode"]),
						Action = ((!Exists(Function.ToString(dt.Rows[i]["TrnspCode"]), sessionId)) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
						BodyObject = empty
					});
				}
				HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					Logging.Write("ERROR", "Save Shipping type error: " + httpResult.Message);
					return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
				}
				string text = Function.ToString(httpResult.Content);
				Logging.Write("WATCH", "Save item group Success: " + string.Join(",", text));
				return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", text));
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool Exists(string p, string sessionId)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(p))
			{
				return false;
			}
			return !string.IsNullOrEmpty(ExecuteScarlar("select 1 from OSHP where \"TrnspCode\" = docEntry;", CommandType.Text, new SqlParameter("@docEntry", p)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	protected override string Mapping(DataRow doc, EnumCommandType commandType)
	{
		string text = "";
		try
		{
			text += "{";
			text += Function.GetJsonString("Name", Function.ToString(doc["TrnspName"]));
			text += Function.GetJsonString("Website", Function.ToString(doc["WebSite"]));
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
