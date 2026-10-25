using System.Data;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Services.Interfaces.ProductionAddon;

public interface IProductionPlanning
{
	DataTable GetDataRoutingByMOCode(string MOCode);

	DataTable GetReceiptByMOCode(string MOCode);

	DataTable GetAttachmentByMOCode(string MOCode);

	int GetMaxMOCode();

	DataTable GetUomByItemCode(string itemcode);

	DataTable GetMaxMONO();

	string Save(DataSet ds);

	DataTable GetMaxLineNumByMONo(string MONo);

	DataTable GetTableSQL();

	DataTable Search(ProductionPlanningImpl plSearch);

	DataTable GetReceipt(int moNo);

	DataTable GetDocRecpt(int MONo);

	DataTable GetPlan(int type);

	DataTable GetRelease();

	DataTable APZ_ProductionAddOn_GetRoutingByRouCode(string Roucode);

	DataTable APZ_ProductionPlanning_GetDataReceiptByMOCode(DataTable dta);

	string CheckControlKey(string keyCode);

	string GetCtrlKeyByResCode(string resCode);

	DataTable GetBatchByItemCode(string itemcode);

	DataTable APZ_ProductionAddon_ProductionPlanning_GetFromAPZ_PPL1ByMoNoToCreatPlan(int MONo);

	DataTable GetClosed();

	DataTable APZ_ProductionPlanning_GetBatchCompleteByBatIssue();

	DataTable ChooseWareHouseByItemCode(string itemcode);
}
