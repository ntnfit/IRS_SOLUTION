using System.Data;

namespace Apzon.Services.Interfaces.Inventory.InventoryReport;

public interface IInventoryReport
{
	DataSet StockMovementSummary(DataTable dt);
}
