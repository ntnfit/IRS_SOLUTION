using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IWBSTemplateClient
{
	Task<DataTable> LoadData(string code, string fromDate, string toDate);

	Task<DataSet> GetDataById(string code);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string code);

	Task<DataTable> FindNextLastFrist(int id, int type);
}
