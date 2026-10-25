using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlForecastRepository : ForecastRepository
{
	public SqlForecastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
