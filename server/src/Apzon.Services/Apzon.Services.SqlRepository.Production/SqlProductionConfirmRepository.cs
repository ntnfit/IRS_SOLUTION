using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlProductionConfirmRepository : ProductionConfirmRepository
{
	public SqlProductionConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
