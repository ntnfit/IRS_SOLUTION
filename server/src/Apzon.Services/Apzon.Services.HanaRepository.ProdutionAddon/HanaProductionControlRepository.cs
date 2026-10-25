using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaProductionControlRepository : ProductionControlRepository
{
	public HanaProductionControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
