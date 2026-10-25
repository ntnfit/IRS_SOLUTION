using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlSalesForeCastRepository : SalesForeCastRepository
{
	public SqlSalesForeCastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
