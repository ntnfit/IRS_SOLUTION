using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaPackingBatchRepository : PackingBatchRepository
{
	public HanaPackingBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
