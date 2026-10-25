using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory;

public class InventoryInWareHouseController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] DataTable dt)
	{
		return UnitOfWork.InventoryInWareHouse.Search(dt);
	}

	[HttpGet]
	public DataTable LoadComboItemGroup()
	{
		return UnitOfWork.InventoryInWareHouse.LoadComboItemGroup();
	}

	[HttpGet]
	public DataTable LoadComboWareHouse()
	{
		return UnitOfWork.InventoryInWareHouse.LoadComboWareHouse();
	}
}
