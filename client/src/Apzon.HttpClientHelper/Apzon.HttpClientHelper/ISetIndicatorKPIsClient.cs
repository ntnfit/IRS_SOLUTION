using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ISetIndicatorKPIsClient
{
	Task<DataTable> LoadData(string code, string name);

	Task<DataTable> GetDataById(string code);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> Update(DataSet ds);

	Task<string> DeleteDocs(string code);
}
