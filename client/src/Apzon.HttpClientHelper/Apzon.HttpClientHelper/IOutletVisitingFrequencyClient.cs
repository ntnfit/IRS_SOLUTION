using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IOutletVisitingFrequencyClient
{
	Task<DataTable> GetData(string userName, int branch);

	Task<HttpResult> Save(DataTable dt);
}
