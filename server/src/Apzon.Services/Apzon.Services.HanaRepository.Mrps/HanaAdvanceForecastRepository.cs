using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaAdvanceForecastRepository : AdvanceForecastRepository
{
	public HanaAdvanceForecastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
