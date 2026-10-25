using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPIScopeRepository : KPIScopeRepository
{
	public HanaKPIScopeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
