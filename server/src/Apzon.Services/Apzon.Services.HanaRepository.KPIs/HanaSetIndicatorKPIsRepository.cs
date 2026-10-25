using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaSetIndicatorKPIsRepository : SetIndicatorKPIsRepository
{
	public HanaSetIndicatorKPIsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
