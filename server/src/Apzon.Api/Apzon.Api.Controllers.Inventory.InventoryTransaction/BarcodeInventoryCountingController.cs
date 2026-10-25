using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class BarcodeInventoryCountingController : BaseApiController
{
	[HttpGet]
	public DataSet GetDataStructure()
	{
		return UnitOfWork.BarcodeInventoryCounting.GetDataStructure();
	}

	[HttpPost]
	public DataTable SearchDoc([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.BarcodeInventoryCounting.SearchDoc(dtSearch);
	}

	[HttpGet]
	public DataTable GetDataLineByDocEntry(int docEntry)
	{
		return UnitOfWork.BarcodeInventoryCounting.GetDataLineByDocEntry(docEntry);
	}

	[HttpGet]
	public int GetNewDocEntry()
	{
		return UnitOfWork.BarcodeInventoryCounting.GetNewDocEntry();
	}

	[HttpGet]
	public DataTable GetNextLastData(Enums.NavigateType type, int docEntry)
	{
		return UnitOfWork.BarcodeInventoryCounting.GetNextLastData(type, docEntry);
	}

	[HttpGet]
	public DataTable GetDataByBarcode(string barcodeString, string whsCode, string binCode)
	{
		return UnitOfWork.BarcodeInventoryCounting.GetDataByBarcode(barcodeString, whsCode, binCode);
	}

	[HttpPost]
	public HttpResult AddNew([FromBody] DataSet dsSave)
	{
		return UnitOfWork.BarcodeInventoryCounting.AddNew(dsSave);
	}

	[HttpPost]
	public HttpResult UpdateData([FromBody] DataSet dsSave)
	{
		return UnitOfWork.BarcodeInventoryCounting.UpdateData(dsSave);
	}

	[HttpPost]
	public string Delete([FromBody] int docEntry)
	{
		return UnitOfWork.BarcodeInventoryCounting.Delete(docEntry);
	}
}
