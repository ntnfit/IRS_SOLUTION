using Apzon.Services.Repository.Retail;

namespace Apzon.Services.SqlRepository.Retail;

public class SqlRetailRepository : RetailRepository
{
	public SqlRetailRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
