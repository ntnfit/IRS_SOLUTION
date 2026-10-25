using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.DMS;

public class DMSReportController : BaseApiController
{
	[HttpGet]
	public HttpResult SalesCoverage()
	{
		return UnitOfWork.DMSReport.SavesCoverage();
	}

	[HttpGet]
	public DataTable GetMarkers()
	{
		return UnitOfWork.DMSReport.GetMarkers();
	}
}
