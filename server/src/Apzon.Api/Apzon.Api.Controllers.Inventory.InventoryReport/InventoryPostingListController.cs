using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory.InventoryReport;

public class InventoryPostingListController : BaseApiController
{
	[HttpPost]
	public DataTable GetData([FromBody] DataTable dt)
	{
		return UnitOfWork.InventoryPostingList.GetData(dt);
	}
}
