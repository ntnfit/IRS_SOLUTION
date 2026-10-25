using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IAdditionalGeneralSettingsClient
{
	Task<DataSet> GetData();

	Task<HttpResult> Save(DataSet ds);

	Task<string> GetDefaultLng();
}
