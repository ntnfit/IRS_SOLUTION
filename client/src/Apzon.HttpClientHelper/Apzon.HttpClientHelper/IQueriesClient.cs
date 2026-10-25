using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IQueriesClient
{
	Task<DataTable> GetParameters(string qname);

	Task<DataTable> GetColumns(string qName, CommandType cmdType, DataTable prs);

	Task<HttpResult> SaveExtractQuery(DataSet ds);

	Task<DataSet> LoadParameters(int reportId);

	Task<DataTable> ListOfQueryReport();

	Task<DataSet> SubmitQueryReport(DataTable prms);

	Task<string> RemoveExtractQR(string Id);

	Task<DataSet> GetListQueries(string userName, string branch, string filter = "");

	Task<DataTable> GetQueryById(int queryId);

	Task<string> GetQueryText(int queryId);

	Task<DataTable> GetAuthGroup();
}
