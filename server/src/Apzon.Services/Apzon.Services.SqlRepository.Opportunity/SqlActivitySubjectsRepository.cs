using Apzon.Services.Repository.Opportunity;

namespace Apzon.Services.SqlRepository.Opportunity;

public class SqlActivitySubjectsRepository : ActivitySubjectsRepository
{
	public SqlActivitySubjectsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
