using System.Collections.Generic;
using System.Data;

namespace Apzon.Services.Interfaces.DashBoard;

public interface IDashBoard
{
	DataSet GetDataCategories(string userCode);

	DataTable GetDataController(int tabId);

	DataTable GetDataQuery(string sql, string type, DataTable dtPara);

	DataSet GetDataChidController(int p, DataTable dataTable);

	DataTable GetDataObj(int tabId);

	DataTable GetListDashboard();

	List<int> GetListDashboardByUser(string userName);
}
