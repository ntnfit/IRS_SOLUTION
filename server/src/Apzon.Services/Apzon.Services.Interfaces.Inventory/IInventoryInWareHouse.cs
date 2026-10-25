using System.Data;

namespace Apzon.Services.Interfaces.Inventory;

public interface IInventoryInWareHouse
{
	DataTable LoadComboWareHouse();

	DataTable LoadComboItemGroup();

	DataTable Search(DataTable dt);
}
