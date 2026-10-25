using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlInterestLevelRepository : InterestLevelRepository
{
	public SqlInterestLevelRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
