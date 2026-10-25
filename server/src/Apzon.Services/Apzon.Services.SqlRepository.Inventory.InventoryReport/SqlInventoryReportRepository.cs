using Apzon.Services.Repository.Inventory.InventoryReport;

namespace Apzon.Services.SqlRepository.Inventory.InventoryReport;

public class SqlInventoryReportRepository : InventoryReportRepository
{
	public SqlInventoryReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
