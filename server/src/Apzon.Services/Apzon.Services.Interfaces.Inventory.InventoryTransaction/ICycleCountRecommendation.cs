using System.Data;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface ICycleCountRecommendation
{
	DataTable GetListCycleCode();

	DataTable GetListItemCode();

	DataTable GetWarehouse();

	DataTable GetListDataLine(string whs, int fromCycleCode, int toCycleCode, string fromItemCode, string toItemCode);
}
