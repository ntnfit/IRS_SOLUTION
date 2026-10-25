using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IVehicleClient
{
	Task<DataTable> GetList();

	Task<HttpResult> Saves(DataTable dt);

	Task<string> DeleteDocs(string ids);

	Task<DataTable> GetDataType();

	Task<DataTable> GetDataBranch();
}
