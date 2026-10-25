using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlInterestRepository : InterestRepository
{
	public SqlInterestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
