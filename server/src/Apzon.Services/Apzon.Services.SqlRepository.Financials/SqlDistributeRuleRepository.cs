using Apzon.Services.Repository.Financials;

namespace Apzon.Services.SqlRepository.Financials;

public class SqlDistributeRuleRepository : DistributeRuleRepository
{
	public SqlDistributeRuleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
