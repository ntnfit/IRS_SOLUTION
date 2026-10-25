using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory.InventoryReport;

public class InventoryReportController : BaseApiController
{
	[HttpPost]
	public DataSet StockMovementSummary([FromBody] DataTable dt)
	{
		return UnitOfWork.InventoryReport.StockMovementSummary(dt);
	}
}
