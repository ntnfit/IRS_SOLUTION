using Apzon.Services.Repository.Sales;

namespace Apzon.Services.SqlRepository.Sales;

public class SqlAtpRepository : AtpRepository
{
	public SqlAtpRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
