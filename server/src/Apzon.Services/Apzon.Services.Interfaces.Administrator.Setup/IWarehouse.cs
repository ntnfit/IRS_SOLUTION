using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IWarehouse : IService<WarehouseImpl>
{
	DataTable Cfl();

	List<WarehouseImpl> CflByWhseCode(string whseCode);

	DataTable ShowWarehouse();

	DataTable Search(string whsCode, string whsName, int bPLid);

	int GetMaxCode();

	string DeleteDoc(string whsCode);

	DataTable GetItemOnhandByWarehouseItemCode(string whsCode, string itemCode);

	DataTable GetDefaultTable();

	string Save(DataTable dtObj);

	DataTable CflDocument();

	string RemoveWarehouses(List<string> lstWarehouse);

	DataTable GetAllCode();

	DataTable GetRecord(string position, string whsCode);

	string DeleteWarehouse(string code);

	Common.Table_Code WhsCheckFollowBin(string whsCode);

	Common.Table_Code WhsGetDefBinItem(string whsCode, string itemCode);
}
