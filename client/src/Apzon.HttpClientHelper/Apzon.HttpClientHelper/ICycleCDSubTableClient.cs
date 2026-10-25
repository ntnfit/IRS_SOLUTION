using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ICycleCDSubTableClient
{
	Task<DataTable> GetInventoryCycles();

	Task<DataTable> GetByCycles(int cyclesBy);

	Task<DataTable> GetUsers();

	Task<DataTable> GetByWhsCode(string whsCode);

	Task<DataTable> SearchDataBywhs(string whs, int cycleBy);

	DataTable GetByCycleCode(int cycleCode);

	Task<HttpResult> Saves(DataTable line);
}
