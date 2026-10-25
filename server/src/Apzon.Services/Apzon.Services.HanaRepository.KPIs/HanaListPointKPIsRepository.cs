using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaListPointKPIsRepository : ListPointKPIsRepository
{
	public HanaListPointKPIsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
