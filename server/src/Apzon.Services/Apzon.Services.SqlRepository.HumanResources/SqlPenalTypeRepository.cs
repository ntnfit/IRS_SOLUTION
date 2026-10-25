using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlPenalTypeRepository : PenalTypeRepository
{
	public SqlPenalTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
