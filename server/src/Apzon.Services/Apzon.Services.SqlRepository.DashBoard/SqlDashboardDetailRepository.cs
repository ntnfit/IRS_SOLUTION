using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.SqlRepository.DashBoard;

public class SqlDashboardDetailRepository : DashboardDetailRepository
{
	public SqlDashboardDetailRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
