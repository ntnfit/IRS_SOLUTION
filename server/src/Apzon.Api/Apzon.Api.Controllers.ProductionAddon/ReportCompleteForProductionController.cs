using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

public class ReportCompleteForProductionController : BaseApiController
{
	[HttpPost]
	public DataTable GetDocsByBarCode([FromBody] string batIssue)
	{
		return UnitOfWork.ReportCompleteForProduction.GetDocsByBarCode(batIssue);
	}
}
