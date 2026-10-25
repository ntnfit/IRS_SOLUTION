using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IInventoryCyclesClient
{
	Task<HttpResult> Saves(DataTable dt);

	Task<HttpResult> Update(DataTable dt);

	Task<HttpResult> Delete(DataTable dt);

	Task<DataTable> FindNextLastFrist(int docId, int type);

	Task<int> GetMaxId();
}
