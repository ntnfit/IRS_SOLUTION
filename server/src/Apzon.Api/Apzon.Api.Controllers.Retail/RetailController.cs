using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Retail;

public class RetailController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocByCode(string userName, string branch, string barCode)
	{
		return UnitOfWork.Retail.GetDocByCode(userName, branch, barCode);
	}

	[HttpGet]
	public DataSet GetTransactionDetails(int docEntry)
	{
		return UnitOfWork.Retail.GetTransactionDetails(docEntry);
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		return UnitOfWork.Retail.Save(ds);
	}

	[HttpGet]
	public DataSet GetStruct()
	{
		return UnitOfWork.Retail.GetStruct();
	}

	[HttpGet]
	public DataSet GetReceiptData(int docEntry, int type)
	{
		return UnitOfWork.Retail.GetReceiptData(docEntry, type);
	}

	[HttpGet]
	public DataSet GetSummaryData(string storeCode, int posID)
	{
		return UnitOfWork.Retail.GetSummaryData(storeCode, posID);
	}

	[HttpGet]
	public DataSet GetRefundLines(int docEntry)
	{
		return UnitOfWork.Retail.GetRefundLines(docEntry);
	}

	[HttpGet]
	public DataTable GetAllTaxCodes()
	{
		return UnitOfWork.Retail.GetAllTaxCodes();
	}

	[HttpPost]
	public DataTable SearchTransactions([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.Retail.SearchTransactions(dtSearch);
	}

	[HttpGet]
	public DataTable RetailInventorySearch(string itemCode)
	{
		return UnitOfWork.Retail.RetailInventorySearch(itemCode);
	}

	[HttpPost]
	public string SaveCmdConfig([FromBody] DataTable dtDoc)
	{
		return UnitOfWork.RetailCmdConfig.Save(dtDoc);
	}

	[HttpGet]
	public DataTable LoadCmdConfig()
	{
		return UnitOfWork.RetailCmdConfig.LoadDoc();
	}

	[HttpPost]
	public string SaveClock([FromBody] DataTable dtDoc)
	{
		return UnitOfWork.Retail.SaveClock(dtDoc);
	}

	[HttpGet]
	public DataTable CheckClock(string storeCode, int posID)
	{
		return UnitOfWork.Retail.CheckClock(storeCode, posID);
	}

	[HttpGet]
	public DataTable GetSODocEntry(string storeCode, int posID)
	{
		return UnitOfWork.Retail.GetSODocEntry(storeCode, posID);
	}

	[HttpPost]
	public DataTable GetPromotionsData([FromBody] DataSet ds)
	{
		return UnitOfWork.Retail.GetPromotionsData(ds);
	}
}
