using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.SqlRepository.DashBoard;

internal class SqlMobileDashboardRepository : MobileDashboardRepository
{
	public SqlMobileDashboardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
