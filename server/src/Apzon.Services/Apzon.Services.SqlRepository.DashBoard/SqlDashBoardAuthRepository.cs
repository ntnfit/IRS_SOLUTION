using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.SqlRepository.DashBoard;

public class SqlDashBoardAuthRepository : DashBoardAuthRepository
{
	public SqlDashBoardAuthRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
