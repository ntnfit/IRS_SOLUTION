using Apzon.Services.Repository.DashBoard;

namespace Apzon.Services.HanaRepository.DashBoard;

public class HanaMobileDashboardRepository : MobileDashboardRepository
{
	public HanaMobileDashboardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
