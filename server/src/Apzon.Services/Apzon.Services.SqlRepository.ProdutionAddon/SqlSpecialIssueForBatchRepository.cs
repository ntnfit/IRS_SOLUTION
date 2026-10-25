using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlSpecialIssueForBatchRepository : SpecialIssueForBatchRepository
{
	public SqlSpecialIssueForBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
