using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaSpecialIssueForBatchRepository : SpecialIssueForBatchRepository
{
	public HanaSpecialIssueForBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
