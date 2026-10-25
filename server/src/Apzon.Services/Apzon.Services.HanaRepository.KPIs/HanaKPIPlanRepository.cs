using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPIPlanRepository : KPIPlanRepository
{
	public HanaKPIPlanRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
