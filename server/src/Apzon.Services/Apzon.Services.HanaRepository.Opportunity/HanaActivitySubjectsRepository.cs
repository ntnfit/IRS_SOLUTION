using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.HanaRepository.Opportunity;

public class HanaActivitySubjectsRepository : ActivitySubjectsRepository
{
	public HanaActivitySubjectsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
