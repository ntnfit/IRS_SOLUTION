using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.SqlRepository.DashBoard;

public class SqlDashboardCategoriesRepository : DashboardCategoriesRepository
{
	public SqlDashboardCategoriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
