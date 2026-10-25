using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlGeographyRepository : GeographyRepository
{
	public SqlGeographyRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
