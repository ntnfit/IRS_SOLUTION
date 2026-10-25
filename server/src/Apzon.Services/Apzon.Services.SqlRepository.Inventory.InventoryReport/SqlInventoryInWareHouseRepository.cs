using Apzon.Services.Repository.Inventory.InventoryReport;

namespace Apzon.Services.SqlRepository.Inventory.InventoryReport;

public class SqlInventoryInWareHouseRepository : InventoryInWareHouseRepository
{
	public SqlInventoryInWareHouseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
