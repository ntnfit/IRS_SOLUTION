using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.ProductionAddon;

[ValidateLoginSapExceptionHandling]
public class TinhGiayController : BaseApiController
{
	[HttpGet]
	public DataTable APZ_ProductionAddon_ListWarehouse()
	{
		return UnitOfWork.TinhGiay.APZ_ProductionAddon_ListWarehouse();
	}

	[HttpGet]
	public DataTable APZ_ProductionAddon_ListItemTinhGiay([FromBody] DataTable tbl)
	{
		return UnitOfWork.TinhGiay.APZ_ProductionAddon_ListItemTinhGiay(tbl);
	}

	[HttpPost]
	public DataTable APZ_ProductionAddon_ListCuonPhuHop([FromBody] DataTable dt)
	{
		return UnitOfWork.TinhGiay.APZ_ProductionAddon_ListCuonPhuHop(dt);
	}

	[HttpPost]
	public string APZ_ProductionAddon_SaveDSTinhGiay([FromBody] DataTable dt)
	{
		return UnitOfWork.TinhGiay.APZ_ProductionAddon_SaveDSTinhGiay(dt);
	}

	[HttpPost]
	public DataTable TinhGhepKho([FromBody] DataTable tbl)
	{
		return UnitOfWork.TinhGiay.TinhGhepKho(tbl);
	}

	[HttpPost]
	public DataSet TinhChonGiay([FromBody] Tuple<DataTable, string> model)
	{
		DataTable item = model.Item1;
		string item2 = model.Item2;
		return UnitOfWork.TinhGiay.TinhChonGiay(item, item2);
	}

	[HttpPost]
	public DataSet XemTonKho([FromBody] Tuple<DataTable, string, string> model)
	{
		DataTable item = model.Item1;
		string item2 = model.Item2;
		string item3 = model.Item3;
		return UnitOfWork.TinhGiay.XemTonKho(item, item2, item3);
	}

	[HttpPost]
	public string FilterTinhGiay([FromBody] DataTable dt)
	{
		return UnitOfWork.TinhGiay.FilterTinhGiay(dt);
	}

	[HttpGet]
	public DataTable GetBatchByItemCode(string itemCode)
	{
		return UnitOfWork.TinhGiay.GetBatchByItemCode(itemCode);
	}

	[HttpGet]
	public DataTable GetWarehouseForBatch(int branchId)
	{
		return UnitOfWork.TinhGiay.GetWarehouseForBatch(branchId);
	}
}
