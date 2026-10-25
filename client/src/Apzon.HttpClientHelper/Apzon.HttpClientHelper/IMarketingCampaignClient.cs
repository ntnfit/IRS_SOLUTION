using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IMarketingCampaignClient
{
	Task<DataTable> LoadData(int id, string campaignNo, string startDate, string toDate, string status);

	Task<DataSet> GetDataById(int Id);

	Task<DataTable> GetDataTargetBPById(int Id);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string Id);

	Task<int> GetMaxId();

	Task<DataTable> FindNextLastFrist(int id, int type);

	Task<DataTable> LoadTargetGroup();

	Task<DataTable> LoadTargetGroupDetails(string code);
}
