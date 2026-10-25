using Apzon.Services.Repository.Sales;

namespace Apzon.Services.SqlRepository.Sales;

public class SqlSalesControlRepository : SalesControlRepository
{
	public SqlSalesControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
