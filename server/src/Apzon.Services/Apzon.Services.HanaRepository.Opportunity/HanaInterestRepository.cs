using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaInterestRepository : InterestRepository
{
	public HanaInterestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
