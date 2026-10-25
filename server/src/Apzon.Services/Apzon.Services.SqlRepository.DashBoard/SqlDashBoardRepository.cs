using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.SqlRepository.DashBoard;

public class SqlDashBoardRepository : DashBoardRepository
{
	public SqlDashBoardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
