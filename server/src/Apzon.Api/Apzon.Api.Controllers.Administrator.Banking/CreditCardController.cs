using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Banking;

public class CreditCardController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.CreditCard.LoadDoc();
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
			return BrokerService.GetUnitOfWork(SessionId).CreditCard.Save(dtDoc, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.CreditCard.Save(dtDoc);
	}
}
