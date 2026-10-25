using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.SqlRepository.DashBoard;

internal class SqlDashBoardTabsRepository : DashBoardTabsRepository
{
	public SqlDashBoardTabsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
