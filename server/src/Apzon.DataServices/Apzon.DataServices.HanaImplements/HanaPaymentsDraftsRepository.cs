using System;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaPaymentsDraftsRepository : PaymentsDraftsRepository
{
	public HanaPaymentsDraftsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	protected override HttpResult UpdateData(IncomingPaymentsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(doc);
			return DataService.UpdateObject(ApzObjectTypes.oPaymentsDrafts, sessionId, xmlObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				Message = ex.Message
			};
		}
	}

	protected override HttpResult AddNew(IncomingPaymentsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(doc);
			return DataService.AddObject(ApzObjectTypes.oPaymentsDrafts, sessionId, xmlObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				Message = ex.Message
			};
		}
	}

	private string Mapping(IncomingPaymentsImpl model)
	{
		try
		{
			string text = "{";
			string docType = model.DocType;
			if (docType == "A")
			{
				text += Function.GetJsonString("DocType", "rAccount");
			}
			else
			{
				text = ((!(docType == "S")) ? (text + Function.GetJsonString("DocType", "rCustomer")) : (text + Function.GetJsonString("DocType", "rSupplier")));
			}
			text += Function.GetJsonString("CardCode", Function.ToString(model.CardCode));
			text += Function.GetJsonString("CardName", Function.ToString(model.CardName));
			if (!string.IsNullOrEmpty(model.PayToCode))
			{
				text += Function.GetJsonString("PayToCode", Function.ToString(model.PayToCode));
			}
			text += Function.GetJsonString("Address", Function.ToString(model.Address));
			if (model.ContactPersonCode > 0)
			{
				text += Function.GetJsonString("ContactPersonCode", Function.ParseInt(model.ContactPersonCode));
			}
			if (!string.IsNullOrEmpty(model.DocCurrency))
			{
				text += Function.GetJsonString("DocCurrency", Function.ToString(model.DocCurrency));
			}
			if (model.DocRate > 0m)
			{
				text += Function.GetJsonString("DocRate", Function.ParseDouble(model.DocRate));
			}
			if (!string.IsNullOrEmpty(model.PrjCode))
			{
				text += Function.GetJsonString("ProjectCode", Function.ToString(model.PrjCode));
			}
			if (model.DocNum > 0)
			{
				text += Function.GetJsonString("DocNum", Function.ParseInt(model.DocNum));
			}
			if (model.DocDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("DocDate", model.DocDate.ToString("yyyyMMdd"));
			}
			if (model.DueDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("DueDate", model.DueDate.ToString("yyyyMMdd"));
			}
			if (model.TaxDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("TaxDate", model.TaxDate.ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(model.CounterReference))
			{
				text += Function.GetJsonString("CounterReference", Function.ToString(model.DocEntry));
			}
			text += Function.GetJsonString("Remarks", Function.ToString(model.Comments));
			text += Function.GetJsonString("JournalRemarks", Function.ToString(model.JrnlMemo));
			if (!string.IsNullOrEmpty(model.ControlAccount))
			{
				text += Function.GetJsonString("ControlAccount", Function.ToString(model.ControlAccount));
			}
			if (model.IsPaytoBnk == "Y")
			{
				text += Function.GetJsonString("IsPayToBank", "tYES");
			}
			else
			{
				text += Function.GetJsonString("IsPayToBank", "tNO");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}
}
