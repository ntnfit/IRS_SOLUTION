using Apzon.Services.Repository.Inventory.InventoryReport;

namespace Apzon.Services.SqlRepository.Inventory.InventoryReport;

public class SqlInventoryPostingListRepository : InventoryPostingListRepository
{
	public SqlInventoryPostingListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
