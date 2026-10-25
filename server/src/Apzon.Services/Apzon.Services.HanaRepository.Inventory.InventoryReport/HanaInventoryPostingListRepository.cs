using Apzon.Services.Repository.Inventory.InventoryReport;

namespace Apzon.Services.HanaRepository.Inventory.InventoryReport;

public class HanaInventoryPostingListRepository : InventoryPostingListRepository
{
	public HanaInventoryPostingListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
