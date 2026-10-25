using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IServiceContractTemplateClient
{
	Task<DataTable> SearchData(DataTable dt);

	Task<DataTable> GetDataDoc(int id, Enums.NavigateType type);

	Task<HttpResult> SaveData(DataTable dt);

	Task<HttpResult> DeleteData(DataTable dt);
}
