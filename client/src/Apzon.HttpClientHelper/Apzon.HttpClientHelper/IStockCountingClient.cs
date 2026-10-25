using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IStockCountingClient
{
	Task<DataTable> SearchDataByWarehouse(string whs);

	Task<DataTable> SearchDataByItemCode(string whs, string itemCode);

	Task<DataTable> GetTableLine(int id);

	Task<DataTable> GetNo();

	Task<HttpResult> Saves(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<int> GetNextNumber(int series);

	Task<DataTable> FindNextLastFrist(int docId, int type);

	Task<HttpResult> SaveDraft(DataSet ds);

	Task<DataTable> GetNoDraf();

	Task<int> GetNextNumberDraft(int series);
}
