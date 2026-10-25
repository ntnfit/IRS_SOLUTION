using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.HanaRepository.DashBoard;

public class HanaDashBoardRepository : DashBoardRepository
{
	public HanaDashBoardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
