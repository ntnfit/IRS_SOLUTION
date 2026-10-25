using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ISettingReportClient
{
	Task<DataTable> LoadData(int id, string fromDate, string toDate, string status);

	Task<DataTable> GetAllLineById(int Id);

	Task<DataTable> GetDataChartById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<int> GetNo();

	Task<DataTable> FindNextLastFrist(int id, int type);

	Task<DataTable> GetColumnStoreReport(string code);

	Task<DataTable> GetDefaultDataSourceReport(string code);

	Task<DataTable> GetDataDashboardById(int docEntry, int id);
}
