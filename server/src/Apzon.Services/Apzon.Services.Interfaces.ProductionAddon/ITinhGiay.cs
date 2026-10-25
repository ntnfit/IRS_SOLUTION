using System.Data;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface ITinhGiay
{
	DataTable APZ_ProductionAddon_ListItemTinhGiay(DataTable tbl);

	DataTable APZ_ProductionAddon_ListWarehouse();

	DataTable APZ_ProductionAddon_ListCuonPhuHop(DataTable dt);

	string APZ_ProductionAddon_SaveDSTinhGiay(DataTable dt);

	DataTable GetBatchByItemCode(string itemCode);

	DataTable TinhGhepKho(DataTable dtTinhGiay);

	DataSet TinhChonGiay(DataTable dtGhepKho, string whs);

	DataSet XemTonKho(DataTable dtGhepKho, string branch, string statusQC);

	DataTable GetWarehouseForBatch(int branchId);

	string FilterTinhGiay(DataTable dt);
}
