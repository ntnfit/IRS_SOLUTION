using System.Data;

namespace Apzon.Services.Interfaces.Dashboard;

public interface IDashboardCategories
{
	DataTable GetData();

	string SaveData(DataTable dtCategories);

	string CheckExistDashboardTab(int CatId);
}
