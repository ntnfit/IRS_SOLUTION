using Apzon.Services.Repository.Inventory.InventoryReport;

namespace Apzon.Services.HanaRepository.Inventory.InventoryReport;

public class HanaInventoryReportRepository : InventoryReportRepository
{
	public HanaInventoryReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
