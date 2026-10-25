using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPIGroupRepository : KPIGroupRepository
{
	public HanaKPIGroupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
