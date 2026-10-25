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

public class HanaCreditCardRepository : CreditCardRepository
{
	public HanaCreditCardRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	protected override HttpResult AddNew(DataTable data, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			for (int i = 0; i < data.Rows.Count; i++)
			{
				if (!Exist(Function.ParseInt(data.Rows[i]["CreditCard"])))
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						BodyObject = Mapping(data.Rows[i]),
						ObjectType = ApzObjectTypes.oCreditCards,
						OutObjectKey = true
					});
				}
				else
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						BodyObject = Mapping(data.Rows[i]),
						ObjectType = ApzObjectTypes.oCreditCards,
						OutObjectKey = true,
						Key = Function.ParseInt(data.Rows[i]["CreditCard"])
					});
				}
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string Mapping(DataRow data)
	{
		string text = "{";
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(data["CardName"])))
			{
				text += Function.GetJsonString("CreditCardName", Function.ToString(data["CardName"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["AcctCode"])))
			{
				text += Function.GetJsonString("GLAccount", Function.ToString(data["AcctCode"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["Phone"])))
			{
				text += Function.GetJsonString("Telephone", Function.ToString(data["Phone"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["CompanyId"])))
			{
				text += Function.GetJsonString("CompanyID", Function.ToString(data["CompanyId"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["Country"])))
			{
				text += Function.GetJsonString("CountryCode", Function.ToString(data["Country"]));
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
