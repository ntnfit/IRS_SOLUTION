using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IThirdPartyTokenClient
{
	Task<DataTable> LoadTokens(string tokenName, string remarks, string status);

	Task<HttpResult> SaveTokens(DataTable dt, string action);

	Task<HttpResult> GetMaxAbsEntry();

	Task<HttpResult> DeleteToken(int entry);
}
