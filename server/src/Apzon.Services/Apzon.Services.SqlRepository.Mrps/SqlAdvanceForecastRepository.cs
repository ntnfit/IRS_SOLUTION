using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlAdvanceForecastRepository : AdvanceForecastRepository
{
	public SqlAdvanceForecastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
