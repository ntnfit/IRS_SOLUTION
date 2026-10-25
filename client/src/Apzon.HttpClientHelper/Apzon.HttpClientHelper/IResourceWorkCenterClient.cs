using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IResourceWorkCenterClient
{
	Task<DataTable> GetDataResourceWorkCenter();

	Task<DataSet> GetDataOriginal();

	Task<DataTable> GetDataDoc(string resCode, Enums.NavigateType type);

	Task<DataTable> SearchResourceWorkCenter(DataTable dtSearch);

	Task<HttpResult> SaveData(DataTable dt);

	Task<HttpResult> DeleteResource(DataTable dt);

	Task<DataTable> GetDataWorkCenterByCode(string code);
}
