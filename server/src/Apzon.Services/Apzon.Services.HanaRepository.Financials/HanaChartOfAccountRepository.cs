using Apzon.Services.Repository.Financials;

namespace Apzon.Services.HanaRepository.Financials;

public class HanaChartOfAccountRepository : ChartOfAccountRepository
{
	public HanaChartOfAccountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
