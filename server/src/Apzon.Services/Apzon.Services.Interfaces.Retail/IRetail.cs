using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Retail;

public interface IRetail
{
	DataTable GetDocByCode(string userName, string branch, string barCode);

	DataTable SearchTransactions(DataTable dtSearch);

	DataSet GetTransactionDetails(int docEntry);

	HttpResult Save(DataSet ds);

	DataSet GetStruct();

	DataTable RetailInventorySearch(string itemCode);

	DataSet GetReceiptData(int docEntry, int type);

	DataSet GetSummaryData(string storeCode, int posID);

	DataSet GetRefundLines(int docEntry);

	DataTable GetAllTaxCodes();

	DataTable CheckClock(string storeCode, int posID);

	string SaveClock(DataTable dt);

	DataTable GetSODocEntry(string storeCode, int posID);

	DataTable GetPromotionsData(DataSet ds);
}
