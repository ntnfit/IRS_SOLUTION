using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class InventoryCountingController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData(int id, string whs, string startFrom, string startTo, string status)
	{
		return UnitOfWork.InventoryCounting.LoadData(id, whs, startFrom, startTo, status);
	}

	[HttpGet]
	public DataSet GetDataById(int Id)
	{
		return UnitOfWork.InventoryCounting.GetDataById(Id);
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		return UnitOfWork.InventoryCounting.Save(ds, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataSet ds)
	{
		return UnitOfWork.InventoryCounting.Save(ds, "U");
	}

	[HttpGet]
	public string CheckBatchSerNum(string itemCode)
	{
		return UnitOfWork.InventoryCounting.CheckBatchSerNum(itemCode);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.InventoryCounting.GetMaxDocEntry();
	}

	[HttpPost]
	public HttpResult UpdateStatus([FromBody] Tuple<int, string> model)
	{
		return UnitOfWork.InventoryCounting.UpdateStatus(model.Item1, model.Item2);
	}

	[HttpGet]
	public DataTable FindNextLastFrist(int docId, int type)
	{
		return UnitOfWork.InventoryCounting.FindNextLastFrist(docId, type);
	}

	[HttpGet]
	public DataTable GetOnHandByItem(string itemCode, string whs)
	{
		return UnitOfWork.InventoryCounting.GetOnHandByItem(itemCode, whs);
	}
}
