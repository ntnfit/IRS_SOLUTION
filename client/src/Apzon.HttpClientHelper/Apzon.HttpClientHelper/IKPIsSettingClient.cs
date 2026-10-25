using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IKPIsSettingClient
{
	Task<DataSetResult> GetKPIsSetting();

	Task<DataSetResult> GetDetail(int absEntry);

	Task<DataTable> LoadCopySetting(int absEntry);

	Task<HttpResult> Add(DataSet ds);

	Task<HttpResult> SaveCopySetting(DataTable dt);

	Task<HttpResult> Update(DataSet ds);

	Task<HttpResult> Delete(DataSet ds);
}
