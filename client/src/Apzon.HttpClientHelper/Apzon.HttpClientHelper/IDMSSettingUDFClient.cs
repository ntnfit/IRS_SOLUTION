using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IDMSSettingUDFClient
{
	Task<HttpResult> GetData(string tableName, int menuId);

	Task<HttpResult> Update(DataTable dt);
}
