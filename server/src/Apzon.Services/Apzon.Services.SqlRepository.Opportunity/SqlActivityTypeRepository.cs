using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlActivityTypeRepository : ActivityTypeRepository
{
	public SqlActivityTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
