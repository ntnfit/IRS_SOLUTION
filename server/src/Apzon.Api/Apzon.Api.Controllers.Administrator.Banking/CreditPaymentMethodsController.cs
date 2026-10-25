using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Administrator.Banking;

public class CreditPaymentMethodsController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.CreditCardPaymentMethods.LoadDoc();
	}

	[HttpGet]
	public DataTable GetCreditCards()
	{
		return UnitOfWork.CreditCardPaymentMethods.GetCreditCards();
	}

	[HttpGet]
	public DataTable GetPaymentCodes()
	{
		return UnitOfWork.CreditCardPaymentMethods.GetPaymentCodes();
	}

	[HttpPost]
	public string Save([FromBody] DataTable dtDoc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "";
			}
			return BrokerService.GetUnitOfWork(SessionId).CreditPaymentMethods.Save(dtDoc, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.CreditCardPaymentMethods.Save(dtDoc);
	}

	[HttpPost]
	public string DeleteLine([FromBody] int crTypeCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.CreditPaymentMethods.DeleteLine(crTypeCode, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.CreditCardPaymentMethods.DeleteLine(crTypeCode);
		}
		return "You must Login SAP to Save";
	}
}
