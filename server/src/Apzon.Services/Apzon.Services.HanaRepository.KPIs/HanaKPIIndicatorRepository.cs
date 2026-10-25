using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPIIndicatorRepository : KPIIndicatorRepository
{
	public HanaKPIIndicatorRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
