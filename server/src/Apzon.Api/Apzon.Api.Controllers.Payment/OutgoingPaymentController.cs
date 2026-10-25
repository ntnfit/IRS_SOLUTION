using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;

namespace Apzon.Api.Controllers.Payment;

public class OutgoingPaymentController : BaseApiController
{
	[HttpPost]
	public HttpResult SaveDoc([FromBody] IncomingPaymentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			return BrokerService.GetUnitOfWork(SessionId).Payments.Save(ApzObjectTypes.oOutgoingPayment, doc, SessionUserInfo.SessionDiServer, isDraft: false);
		}
		return new HttpResult(Enums.MessageCode.Success, UnitOfWork.OutgoingPayment.SaveDoc(doc));
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
			return BrokerService.GetUnitOfWork(SessionId).Payments.Save(ApzObjectTypes.oOutgoingPayment, doc, SessionUserInfo.SessionDiServer, isDraft: true);
		}
		return new HttpResult(Enums.MessageCode.Error, "Function not support");
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string code)
	{
		return UnitOfWork.OutgoingPayment.DeleteDoc(code);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveUDF([FromBody] List<Common.Table_ID_Code> lst)
	{
		return UnitOfWork.OutgoingPayment.SaveUDF(lst);
	}

	[HttpGet]
	public DataTable ShowOutgoingPayment()
	{
		return UnitOfWork.OutgoingPayment.ShowOutgoingPayment();
	}

	[HttpGet]
	public DataTable GetOutgoingPayment(string docentry)
	{
		return UnitOfWork.OutgoingPayment.GetOutgoingPayment(docentry);
	}

	[HttpGet]
	public DataTable GetAPInvoice(string iCardCode, string iStatus)
	{
		return UnitOfWork.OutgoingPayment.GetAPInvoice(iCardCode, iStatus);
	}

	[HttpGet]
	public DataTable GetARInvoiceByOutgoingPayment(string docEntry)
	{
		return UnitOfWork.OutgoingPayment.GetARInvoiceByOutgoingPayment(docEntry);
	}

	[HttpGet]
	public DataTable GetAPInvoiceByOutgoingPayment(string docEntry)
	{
		return UnitOfWork.OutgoingPayment.GetAPInvoiceByOutgoingPayment(docEntry);
	}

	[HttpGet]
	public DataTable GetPurposeOutgoingPayment(string docEntry)
	{
		return UnitOfWork.OutgoingPayment.GetPurposeOutgoingPayment(docEntry);
	}

	[HttpGet]
	public DataTable GetAccountPayment(string docNums)
	{
		return UnitOfWork.OutgoingPayment.GetAccountPayment(docNums);
	}

	[HttpGet]
	public DataTable GetVatGroupO()
	{
		return UnitOfWork.VatGroups.GetVatGroupO();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.OutgoingPayment.GetMaxCode();
	}

	[HttpGet]
	public DataTable LoadUDFData(string docEntry)
	{
		return UnitOfWork.OutgoingPayment.LoadUDFData(docEntry);
	}

	[HttpGet]
	public DataTable GetNext(string docEntry)
	{
		return UnitOfWork.OutgoingPayment.GetNext(docEntry);
	}

	[HttpGet]
	public DataTable GetPrevious(string docEntry)
	{
		return UnitOfWork.OutgoingPayment.GetPrevious(docEntry);
	}

	[HttpGet]
	public DataTable GetFirst()
	{
		return UnitOfWork.OutgoingPayment.GetFirst();
	}

	[HttpGet]
	public DataTable GetLast()
	{
		return UnitOfWork.OutgoingPayment.GetLast();
	}

	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.OutgoingPayment.GetAll();
	}
}
