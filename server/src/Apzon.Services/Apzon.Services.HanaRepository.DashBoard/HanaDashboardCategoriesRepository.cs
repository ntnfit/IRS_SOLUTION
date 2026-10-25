using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.HanaRepository.DashBoard;

public class HanaDashboardCategoriesRepository : DashboardCategoriesRepository
{
	public HanaDashboardCategoriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
