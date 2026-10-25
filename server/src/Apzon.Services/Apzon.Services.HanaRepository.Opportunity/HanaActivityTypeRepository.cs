using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaActivityTypeRepository : ActivityTypeRepository
{
	public HanaActivityTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
