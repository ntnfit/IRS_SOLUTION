using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlSalesEmployeeBuyerRepository : SalesEmployeeBuyerRepository
{
	public SqlSalesEmployeeBuyerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
