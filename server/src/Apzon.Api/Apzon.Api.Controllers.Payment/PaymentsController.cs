using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class PaymentsController : BaseApiController
{
	[HttpGet]
	public DataTable GetById(int docEntry)
	{
		return UnitOfWork.Payments.GetById(docEntry);
	}
}
