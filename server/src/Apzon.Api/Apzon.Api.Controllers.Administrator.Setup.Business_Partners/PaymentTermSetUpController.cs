using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Administrator.Setup.Business_Partners;

public class PaymentTermSetUpController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.PaymentTerm.LoadDoc();
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dtDoc)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return "INVALIDSESSION";
		}
		IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
		if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
		{
			return unitOfWork.PaymentTermSetUp.Save(dtDoc, SessionUserInfo.SessionDiServer);
		}
		return "INVALIDSESSION";
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int groupNum)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			return "INVALIDSESSION";
		}
		IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
		if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
		{
			return unitOfWork.PaymentTermSetUp.Remove(groupNum, SessionUserInfo.SessionDiServer);
		}
		return "INVALIDSESSION";
	}

	[HttpGet]
	public DataTable GetPaymentTermInfo(int groupNum)
	{
		return UnitOfWork.PaymentTerm.GetPaymentTermInfo(groupNum);
	}
}
