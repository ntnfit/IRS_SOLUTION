using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Setup.Inventory;

public interface IWHSubSetGenPre
{
	DataTable GetAllWarehouseSublevel();

	DataTable GetWarehouseSublevelByFldAbs(long abs);

	DataTable GetAllBinLocations();
}
