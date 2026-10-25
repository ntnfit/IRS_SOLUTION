using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlIssueForProductionRepository : IssueForProductionRepository
{
	public SqlIssueForProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
