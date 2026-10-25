using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Util;

public class AdvancePeriodClosingController : BaseApiController
{
	[HttpGet]
	public DataTable GetPeriod()
	{
		return UnitOfWork.AdvancePeriodClosing.GetPeriod();
	}
}
