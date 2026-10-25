using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface ICycleCountRecommendationClient
{
	Task<DataTable> GetListCycleCode();

	Task<DataTable> GetListItemCode();

	Task<DataTable> GetWarehouse();

	Task<DataTable> GetListDataLine(string whs, int fromCycleCode, int toCycleCode, string fromItemCode, string toItemCode);
}
