using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;

namespace Apzon.Api.Controllers.Payment;

public class IncomingPaymentController : BaseApiController
{
	[HttpPost]
	public HttpResult SaveDoc([FromBody] IncomingPaymentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).Payments.Save(ApzObjectTypes.oIncomingPayment, doc, SessionUserInfo.SessionDiServer, isDraft: false);
		}
		return new HttpResult(Enums.MessageCode.Success, UnitOfWork.IncomingPayment.SaveDoc(doc));
	}

	[HttpPost]
	public HttpResult SaveAsDraft([FromBody] IncomingPaymentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).Payments.Save(ApzObjectTypes.oIncomingPayment, doc, SessionUserInfo.SessionDiServer, isDraft: true);
		}
		return new HttpResult(Enums.MessageCode.Error, "Function not support");
	}

	[HttpPost]
	public HttpResult SaveListDoc([FromBody] List<IncomingPaymentsImpl> docs)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).Payments.SaveListDoc(ApzObjectTypes.oIncomingPayment, docs, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult();
	}

	[HttpPost]
	public HttpResult SaveDraftToDocument([FromBody] int docEntry)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.Payments.SaveDraftToDocument(docEntry, SessionUserInfo.SessionDiServer);
				}
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveUDF([FromBody] List<Common.Table_ID_Code> lst)
	{
		return UnitOfWork.IncomingPayment.SaveUDF(lst);
	}

	[HttpPost]
	public DataSet GetListInvoice(DataTable dtSearch)
	{
		try
		{
			return UnitOfWork.IncomingPayment.GetListInvoice(dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet GetListPayment(DataTable dtSearch)
	{
		try
		{
			return UnitOfWork.IncomingPayment.GetListPayment(dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetByInvoiceNo(string barcode)
	{
		try
		{
			return UnitOfWork.IncomingPayment.GetByInvoiceNo(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable ShowIncomingPayment()
	{
		return UnitOfWork.IncomingPayment.ShowIncomingPayment();
	}

	[HttpGet]
	public DataTable GetIncomingPayment(string docentry)
	{
		return UnitOfWork.IncomingPayment.GetIncomingPayment(docentry);
	}

	[HttpGet]
	public DataTable GetARInvoice(string iCardCode, string iStatus)
	{
		return UnitOfWork.IncomingPayment.GetARInvoice(iCardCode, iStatus);
	}

	[HttpGet]
	public DataTable GetARInvoiceByIncomingPayment(string docEntry)
	{
		return UnitOfWork.IncomingPayment.GetARInvoiceByIncomingPayment(docEntry);
	}

	[HttpGet]
	public DataTable GetAPInvoiceByIncomingPayment(string docEntry)
	{
		return UnitOfWork.IncomingPayment.GetAPInvoiceByIncomingPayment(docEntry);
	}

	[HttpGet]
	public DataTable GetPurposeIncomingPayment(string docEntry)
	{
		return UnitOfWork.IncomingPayment.GetPurposeIncomingPayment(docEntry);
	}

	[HttpGet]
	public DataTable GetAccountPayment(string docNums)
	{
		return UnitOfWork.IncomingPayment.GetAccountPayment(docNums);
	}

	[HttpGet]
	public DataTable GetVatGroupO()
	{
		return UnitOfWork.VatGroups.GetVatGroupO();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.IncomingPayment.GetMaxCode();
	}

	[HttpGet]
	public DataTable LoadUDFData(string docEntry)
	{
		return UnitOfWork.IncomingPayment.LoadUDFData(docEntry);
	}

	[HttpGet]
	public DataTable GetNext(string docEntry)
	{
		return UnitOfWork.IncomingPayment.GetNext(docEntry);
	}

	[HttpGet]
	public DataTable GetPrevious(string docEntry)
	{
		return UnitOfWork.IncomingPayment.GetPrevious(docEntry);
	}

	[HttpGet]
	public DataTable GetFirst()
	{
		return UnitOfWork.IncomingPayment.GetFirst();
	}

	[HttpGet]
	public DataTable GetLast()
	{
		return UnitOfWork.IncomingPayment.GetLast();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được cập nhật!";
		}
		return UnitOfWork.IncomingPayment.DeleteDoc(code);
	}

	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.IncomingPayment.GetAll();
	}
}
