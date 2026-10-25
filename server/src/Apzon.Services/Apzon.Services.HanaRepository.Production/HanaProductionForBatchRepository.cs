using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaProductionForBatchRepository : ProductionForBatchRepository
{
	public HanaProductionForBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
