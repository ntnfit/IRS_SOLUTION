using Apzon.Services.Repository.Retail;

namespace Apzon.Services.SqlRepository.Retail;

public class SqlRetailCmdConfigRepository : RetailCmdConfigRepository
{
	public SqlRetailCmdConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
