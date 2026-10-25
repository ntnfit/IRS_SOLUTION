using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Payment;

public class ChartOfAccountPurposeController : BaseApiController
{
	[HttpGet]
	public DataTable Cfl()
	{
		return UnitOfWork.IncomingPayment.Cfl();
	}
}
