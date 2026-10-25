using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Production_Addon;

namespace Apzon.Api.Controllers.ProductionAddon;

public class ProductionPlanningController : BaseApiController
{
	[HttpGet]
	public int GetMaxMOCode()
	{
		return UnitOfWork.ProductionPlanning.GetMaxMOCode();
	}

	[HttpPost]
	public DataTable GetUomByItemCode([FromBody] string itemcode)
	{
		return UnitOfWork.ProductionPlanning.GetUomByItemCode(itemcode);
	}

	[HttpPost]
	public string Save([FromBody] DataSet ds)
	{
		return UnitOfWork.ProductionPlanning.Save(ds);
	}

	[HttpGet]
	public DataTable GetMaxMONO()
	{
		return UnitOfWork.ProductionPlanning.GetMaxMONO();
	}

	[HttpPost]
	public DataTable GetMaxLineNumByMONo(string MONo)
	{
		return UnitOfWork.ProductionPlanning.GetMaxLineNumByMONo(MONo);
	}

	[HttpGet]
	public DataTable GetTableSQL()
	{
		return UnitOfWork.ProductionPlanning.GetTableSQL();
	}

	[HttpPost]
	public DataTable Search([FromBody] ProductionPlanningImpl plSearch)
	{
		return UnitOfWork.ProductionPlanning.Search(plSearch);
	}

	[HttpPost]
	public DataTable GetReceipt([FromBody] int moNo)
	{
		return UnitOfWork.ProductionPlanning.GetReceipt(moNo);
	}

	[HttpPost]
	public DataTable GetDocRecpt([FromBody] int MONo)
	{
		return UnitOfWork.ProductionPlanning.GetDocRecpt(MONo);
	}

	[HttpPost]
	public DataTable GetPlan([FromBody] int type)
	{
		return UnitOfWork.ProductionPlanning.GetPlan(type);
	}

	[HttpGet]
	public DataTable GetRelease()
	{
		return UnitOfWork.ProductionPlanning.GetRelease();
	}

	[HttpPost]
	public DataTable APZ_ProductionPlanning_GetDataReceiptByMOCode(DataTable dta)
	{
		return UnitOfWork.ProductionPlanning.APZ_ProductionPlanning_GetDataReceiptByMOCode(dta);
	}

	[HttpPost]
	public string CheckControlKey([FromBody] string keyCode)
	{
		return UnitOfWork.ProductionPlanning.CheckControlKey(keyCode);
	}

	[HttpPost]
	public string GetCtrlKeyByResCode([FromBody] string resCode)
	{
		return UnitOfWork.ProductionPlanning.GetCtrlKeyByResCode(resCode);
	}

	[HttpPost]
	public DataTable GetBatchByItemCode([FromBody] string itemcode)
	{
		return UnitOfWork.ProductionPlanning.GetBatchByItemCode(itemcode);
	}

	[HttpPost]
	public DataTable ChooseWareHouseByItemCode([FromBody] string itemcode)
	{
		return UnitOfWork.ProductionPlanning.ChooseWareHouseByItemCode(itemcode);
	}

	[HttpPost]
	public DataTable APZ_ProductionAddon_ProductionPlanning_GetFromAPZ_PPL1ByMoNoToCreatPlan([FromBody] int MONo)
	{
		return UnitOfWork.ProductionPlanning.APZ_ProductionAddon_ProductionPlanning_GetFromAPZ_PPL1ByMoNoToCreatPlan(MONo);
	}

	[HttpGet]
	public DataTable GetClosed()
	{
		return UnitOfWork.ProductionPlanning.GetClosed();
	}

	[HttpGet]
	public DataTable APZ_ProductionPlanning_GetBatchCompleteByBatIssue()
	{
		return UnitOfWork.ProductionPlanning.APZ_ProductionPlanning_GetBatchCompleteByBatIssue();
	}
}
