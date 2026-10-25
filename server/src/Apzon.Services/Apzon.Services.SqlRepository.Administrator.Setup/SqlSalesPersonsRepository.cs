using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlSalesPersonsRepository : SalesPersonsRepository
{
	public SqlSalesPersonsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
