using Apzon.Services.Repository.Util;

namespace Apzon.Services.SqlRepository.Util;

public class SqlAdvancePeriodClosingRepository : AdvancePeriodClosingRepository
{
	public SqlAdvancePeriodClosingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
