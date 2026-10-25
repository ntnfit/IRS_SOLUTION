using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IKPIGroupClient
{
	Task<DataSet> Search(DataTable dt);

	Task<DataTable> GetByCode(string code);

	Task<DataSet> GetLinesEmployee(int docentry);

	Task<DataSet> GetLinesIndicator(int docentry);

	Task<DataTable> GetLinesEmployeeByCode(string code);

	Task<DataTable> GetLinesIndicatorByCode(string code, string type = "");

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<HttpResult> Delete(DataSet ds);
}
