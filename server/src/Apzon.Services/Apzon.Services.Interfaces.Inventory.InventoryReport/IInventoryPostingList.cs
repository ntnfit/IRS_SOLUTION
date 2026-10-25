using System.Data;

namespace Apzon.Services.Interfaces.Inventory.InventoryReport;

public interface IInventoryPostingList
{
	DataTable GetData(DataTable dt);
}
