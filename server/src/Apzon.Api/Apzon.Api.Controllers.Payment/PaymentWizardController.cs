using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class PaymentWizardController : BaseApiController
{
	[HttpPost]
	public DataSet GetData([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.PaymentWizard.GetData(dtSearch);
	}

	[HttpPost]
	public DataSet GetPaymentInfo([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.PaymentWizard.GetPaymentInfo(dtSearch);
	}

	[HttpPost]
	public DataSet GetDetails([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.PaymentWizard.GetDetails(dtSearch);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.InPayment.Save(ds);
	}
}
