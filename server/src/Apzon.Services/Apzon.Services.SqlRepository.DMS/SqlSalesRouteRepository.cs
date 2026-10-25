using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlSalesRouteRepository : SalesRouteRepository
{
	public SqlSalesRouteRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
