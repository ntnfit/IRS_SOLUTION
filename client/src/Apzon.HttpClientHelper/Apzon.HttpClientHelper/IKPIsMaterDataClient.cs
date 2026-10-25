using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IKPIsMaterDataClient
{
	Task<DataTable> LoadData(string code, string name, string status);

	Task<HttpResult> Saves(DataTable dt);

	Task<string> DeleteDocs(string ids);

	Task<DataTable> LoadDataFormulaCode();
}
