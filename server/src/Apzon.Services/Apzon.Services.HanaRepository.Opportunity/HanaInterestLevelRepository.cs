using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaInterestLevelRepository : InterestLevelRepository
{
	public HanaInterestLevelRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
