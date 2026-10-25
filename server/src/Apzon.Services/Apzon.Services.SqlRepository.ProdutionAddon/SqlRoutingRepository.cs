using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlRoutingRepository : RoutingRepository
{
	public SqlRoutingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
