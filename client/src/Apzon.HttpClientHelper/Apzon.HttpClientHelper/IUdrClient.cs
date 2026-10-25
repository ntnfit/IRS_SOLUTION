using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IUdrClient
{
	Task<DataSetResult> GetParamByStore(string storeName, string code, string userName, int branch);

	Task<DataSetResult> GetListReport(string userName, int branch);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<HttpResult> Delete(DataSet ds);

	Task<DataTable> GetParamsByCode(string code);

	Task<DataTable> GetQueryByCode(string code);
}
