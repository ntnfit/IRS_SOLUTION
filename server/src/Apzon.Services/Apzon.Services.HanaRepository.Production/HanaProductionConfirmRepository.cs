using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaProductionConfirmRepository : ProductionConfirmRepository
{
	public HanaProductionConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
