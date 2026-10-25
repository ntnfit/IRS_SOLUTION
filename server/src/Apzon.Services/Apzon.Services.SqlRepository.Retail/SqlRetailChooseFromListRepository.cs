using Apzon.Services.Repository.Retail;

namespace Apzon.Services.SqlRepository.Retail;

public class SqlRetailChooseFromListRepository : RetailChooseFromListRepository
{
	public SqlRetailChooseFromListRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
