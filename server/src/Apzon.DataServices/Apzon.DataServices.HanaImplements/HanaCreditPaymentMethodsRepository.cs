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

public class HanaCreditPaymentMethodsRepository : CreditPaymentMethodsRepository
{
	public HanaCreditPaymentMethodsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
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
				if (!Exist(Function.ParseInt(data.Rows[i]["CrTypeCode"])))
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						BodyObject = Mapping(data.Rows[i]),
						ObjectType = ApzObjectTypes.oCreditPaymentMethods,
						OutObjectKey = true
					});
				}
				else
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						BodyObject = Mapping(data.Rows[i]),
						ObjectType = ApzObjectTypes.oCreditPaymentMethods,
						OutObjectKey = true,
						Key = Function.ParseInt(data.Rows[i]["CrTypeCode"])
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

	public override string DeleteLine(int crTypeCode, string sessionId)
	{
		try
		{
			return DataService.RemoveObject(ApzObjectTypes.oCreditPaymentMethods, sessionId, crTypeCode).Message;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private string Mapping(DataRow data)
	{
		string text = "{";
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(data["CrTypeName"])))
			{
				text += Function.GetJsonString("Name", Function.ToString(data["CrTypeName"]));
			}
			if (Function.ParseInt(data["CreditCard"]) > 0)
			{
				text += Function.GetJsonString("AssignedtoCreditCard", Function.ToString(data["CreditCard"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["DueTerms"])))
			{
				text += Function.GetJsonString("PaymentCode", Function.ToString(data["DueTerms"]));
			}
			if (Function.ParseDouble(data["MinCredit"]) > 0.0)
			{
				text += Function.GetJsonString("MinimumCreditAmount", Function.ToString(data["MinCredit"]));
			}
			if (Function.ParseDouble(data["MinToPay"]) > 0.0)
			{
				text += Function.GetJsonString("MinimumPaymentAmount", Function.ToString(data["MinToPay"]));
			}
			if (Function.ParseDouble(data["MaxValid"]) > 0.0)
			{
				text += Function.GetJsonString("MaxQtyWithoutApproval", Function.ToString(data["MaxValid"]));
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["InstalMent"])))
			{
				text += Function.GetJsonString("InstallmentPaymentsPossible", Function.ToString(data["InstalMent"]));
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
