using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaActivityStatusRepository : ActivityStatusRepository
{
	public HanaActivityStatusRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
