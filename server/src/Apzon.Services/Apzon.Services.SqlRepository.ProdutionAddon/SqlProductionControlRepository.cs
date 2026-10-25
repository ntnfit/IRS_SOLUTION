using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlProductionControlRepository : ProductionControlRepository
{
	public SqlProductionControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
