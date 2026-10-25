using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IInventoryCountingRequestClient
{
	Task<DataTable> LoadData(int id, string whs, string startFrom, string startTo);

	Task<HttpResult> Saves(DataTable dt);

	Task<string> DeleteDocs(string ids);

	Task<DataTable> FindNextLastFrist(int docId, int type);
}
