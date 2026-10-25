using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlItemQuotaArrangementRepository : ItemQuotaArrangementRepository
{
	public SqlItemQuotaArrangementRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
