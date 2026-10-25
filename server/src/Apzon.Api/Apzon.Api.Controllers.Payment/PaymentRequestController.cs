using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class PaymentRequestController : BaseApiController
{
	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.PaymentRequest.GetDoc();
	}

	[HttpPost]
	public DataTable Search(DataTable dtSearch)
	{
		return UnitOfWork.PaymentRequest.Search(dtSearch);
	}

	[HttpGet]
	public string Remove(int docNo)
	{
		return UnitOfWork.PaymentRequest.Remove(docNo);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.PaymentRequest.Save(ds);
	}

	[HttpPost]
	public string UpdateStatus([FromBody] DataSet ds)
	{
		return UnitOfWork.PaymentRequest.UpdateStatus(ds);
	}

	[HttpPost]
	public string Approval([FromBody] DataTable dt)
	{
		return UnitOfWork.PaymentRequest.Approval(dt);
	}
}
