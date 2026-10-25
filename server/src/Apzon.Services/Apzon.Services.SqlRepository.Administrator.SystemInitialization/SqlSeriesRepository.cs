using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.SqlRepository.Administrator.SystemInitialization;

public class SqlSeriesRepository : SeriesRepository
{
	public SqlSeriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
