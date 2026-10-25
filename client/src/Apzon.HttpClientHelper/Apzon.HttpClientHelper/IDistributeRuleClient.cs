using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IDistributeRuleClient
{
	Task<DataTable> GetStruct();

	Task<DataTable> GetStructRow();
}
