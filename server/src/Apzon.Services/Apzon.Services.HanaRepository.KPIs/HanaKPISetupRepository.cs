using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPISetupRepository : KPISetupRepository
{
	public HanaKPISetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
