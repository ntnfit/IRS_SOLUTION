using Apzon.Services.Repository.Util;

namespace Apzon.Services.HanaRepository.Util;

public class HanaAdvancePeriodClosingRepository : AdvancePeriodClosingRepository
{
	public HanaAdvancePeriodClosingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
