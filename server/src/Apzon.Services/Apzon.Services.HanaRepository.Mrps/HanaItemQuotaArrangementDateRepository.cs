using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaItemQuotaArrangementDateRepository : ItemQuotaArrangementDateRepository
{
	public HanaItemQuotaArrangementDateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
