using Apzon.Services.Repository.KPIs;

namespace Apzon.Services.HanaRepository.KPIs;

internal class HanaKPIsSettingRepository : KPIsSettingRepository
{
	public HanaKPIsSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
