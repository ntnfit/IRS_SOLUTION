using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlStatesRepository : StatesRepository
{
	public SqlStatesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
