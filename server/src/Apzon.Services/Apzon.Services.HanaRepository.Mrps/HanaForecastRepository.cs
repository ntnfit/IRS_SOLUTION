using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaForecastRepository : ForecastRepository
{
	public HanaForecastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
