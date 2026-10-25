using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPIScoreRepository : KPIScoreRepository
{
	public HanaKPIScoreRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
