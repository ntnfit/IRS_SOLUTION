using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class PaymentMeansController : BaseApiController
{
	[HttpGet]
	public DataTable ShowCreditCardType()
	{
		return UnitOfWork.IncomingPayment.ShowPaymentMeansCreditCardType();
	}

	[HttpGet]
	public DataTable ShowCurrency()
	{
		return UnitOfWork.IncomingPayment.ShowPaymentMeansCurrency();
	}

	[HttpGet]
	public DataTable LoadPaymentMeansByIncoming(string docNum)
	{
		return UnitOfWork.IncomingPayment.LoadPaymentMeansByIncoming(docNum);
	}

	[HttpGet]
	public DataTable LoadPaymentMeansByOutgoing(string docNum)
	{
		return UnitOfWork.OutgoingPayment.LoadPaymentMeansByOutgoing(docNum);
	}
}
