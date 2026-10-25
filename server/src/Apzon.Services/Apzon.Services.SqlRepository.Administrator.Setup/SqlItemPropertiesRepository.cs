using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlItemPropertiesRepository : ItemPropertiesRepository
{
	public SqlItemPropertiesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
