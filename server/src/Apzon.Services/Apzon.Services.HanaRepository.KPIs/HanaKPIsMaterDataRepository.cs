using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPIsMaterDataRepository : KPIsMaterDataRepository
{
	public HanaKPIsMaterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
