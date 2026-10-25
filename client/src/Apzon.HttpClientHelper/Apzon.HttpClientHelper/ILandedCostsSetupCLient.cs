using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ILandedCostsSetupCLient
{
	Task<DataTable> GetLandedCostsSetup();

	Task<string> RemoveLandedCostsSetup(string code);

	Task<HttpResult> SaveLandedCostsSetup(DataTable dt);
}
