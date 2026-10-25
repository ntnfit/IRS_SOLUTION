using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.HanaRepository.DashBoard;

public class HanaDashBoardTabsRepository : DashBoardTabsRepository
{
	public HanaDashBoardTabsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
