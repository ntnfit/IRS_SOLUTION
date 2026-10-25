using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlItemQuotaArrangementDateRepository : ItemQuotaArrangementDateRepository
{
	public SqlItemQuotaArrangementDateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
