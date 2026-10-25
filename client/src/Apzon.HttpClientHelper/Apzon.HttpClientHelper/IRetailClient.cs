using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IRetailClient
{
	Task<DataSet> GetStruct();

	Task<DataTable> LoadCmdConfig();

	Task<string> SaveCmdConfig(DataTable dtDoc);

	Task<DataSet> GetReceiptData(int docEntry, int type);

	Task<DataSet> GetSummaryData(string storeCode, int posID);

	Task<DataSet> GetRefundLines(int docEntry);

	Task<DataTable> GetAllTaxCodes();

	Task<DataTable> CheckClock(string storeCode, int posID);

	Task<DataTable> GetSODocEntry(string storeCode, int posID);

	Task<string> SaveClock(DataTable dtDoc);

	Task<DataTable> GetPromotionsData(DataSet ds);
}
