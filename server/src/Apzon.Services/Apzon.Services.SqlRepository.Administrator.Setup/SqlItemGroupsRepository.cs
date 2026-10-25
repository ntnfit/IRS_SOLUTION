using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlItemGroupsRepository : ItemGroupsRepository
{
	public SqlItemGroupsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
