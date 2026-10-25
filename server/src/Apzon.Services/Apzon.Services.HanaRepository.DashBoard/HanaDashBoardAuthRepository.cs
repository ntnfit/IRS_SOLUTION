using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.HanaRepository.DashBoard;

public class HanaDashBoardAuthRepository : DashBoardAuthRepository
{
	public HanaDashBoardAuthRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
