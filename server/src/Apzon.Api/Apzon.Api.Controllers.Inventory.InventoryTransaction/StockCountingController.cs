using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class StockCountingController : BaseApiController
{
	[HttpGet]
	public DataTable SearchDataByWarehouse(string whs)
	{
		return UnitOfWork.StockCounting.SearchDataByWarehouse(whs);
	}

	[HttpGet]
	public DataTable SearchDataByItemCode(string whs, string itemCode)
	{
		return UnitOfWork.StockCounting.SearchDataByItemCode(whs, itemCode);
	}

	[HttpGet]
	public DataTable GetTableLine(int id)
	{
		return UnitOfWork.StockCounting.GetTableLine(id);
	}

	[HttpGet]
	public DataTable GetNo()
	{
		return UnitOfWork.StockCounting.GetNo();
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataSet ds)
	{
		return UnitOfWork.StockCounting.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.StockCounting.Save(ds, "U");
	}

	[HttpGet]
	public int GetNextNumber(int series)
	{
		return UnitOfWork.StockCounting.GetNextNumber(series);
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int docId, int type)
	{
		return UnitOfWork.StockCounting.FindNextLastFrist(docId, type);
	}

	[HttpPost]
	public HttpResult SaveDraft([FromBody] DataSet ds)
	{
		return UnitOfWork.StockCounting.SaveDraft(ds, "D");
	}

	[HttpGet]
	public DataTable GetNoDraf()
	{
		return UnitOfWork.StockCounting.GetNoDraf();
	}

	[HttpGet]
	public int GetNextNumberDraft(int series)
	{
		return UnitOfWork.StockCounting.GetNextNumberDraft(series);
	}
}
