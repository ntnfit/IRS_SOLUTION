using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class InventoryConfirmController : BaseApiController
{
	[HttpGet]
	public DataSet SearchDataByBarCode(string barcode, string whs)
	{
		return UnitOfWork.InventoryConfirm.SearchDataByBarCode(barcode, whs);
	}

	[HttpGet]
	public DataSet GetLineItem(int docEntry)
	{
		return UnitOfWork.InventoryConfirm.GetLineItem(docEntry);
	}

	[HttpGet]
	public string GetRecord(int docEntry, int type)
	{
		return UnitOfWork.InventoryConfirm.GetRecord(docEntry, type);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.InventoryConfirm.GetMaxDocEntry();
	}
}
