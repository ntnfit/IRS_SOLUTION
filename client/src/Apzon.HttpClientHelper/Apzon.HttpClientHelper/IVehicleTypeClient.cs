using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IVehicleTypeClient
{
	Task<DataTable> GetList();

	Task<HttpResult> Saves(DataTable dt);

	Task<string> DeleteDocs(string ids);
}
