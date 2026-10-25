using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IDashboardClient
{
	Task<DataTable> GetListDashboard();

	Task<List<int>> GetListDashboardByUser(string userName);
}
