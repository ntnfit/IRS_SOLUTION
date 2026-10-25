using Apzon.Services.Repository.Inventory.InventoryReport;

namespace Apzon.Services.HanaRepository.Inventory.InventoryReport;

public class HanaInventoryInWareHouseRepository : InventoryInWareHouseRepository
{
	public HanaInventoryInWareHouseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
