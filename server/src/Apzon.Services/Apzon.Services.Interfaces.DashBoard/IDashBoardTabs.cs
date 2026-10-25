using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DashBoard;

public interface IDashBoardTabs
{
	DataTable GetAllDashBoardTabs();

	DataTable GetControllersByTab(int tabId);

	DataTable GetTabByTabId(int tabId);

	DataTable GetTabByTabName(string tabName);

	DataTable GetAllTabsByCategory(int catId);

	DataTable GetAllDashBoardControllers();

	HttpResult SaveDoc(DataSet ds);

	string DeleteTab(int tabId);

	DataTable GetMaxControllerId();

	DataTable GetMaxTabId();
}
