using System.Data;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IApzBillOfMaterials
{
	DataTable Search(ApzBillOfMaterials bom);

	DataTable SearchLines(ApzBillOfMaterials bom);

	bool CheckExist(string itemCode, string cardCode);

	bool CheckLineExist(string fatherCode, string cardCode, int childNum);

	string SaveBoms(DataSet ds);

	string DeleteBom(ApzBillOfMaterials bom);

	DataTable GetList_BOM();
}
