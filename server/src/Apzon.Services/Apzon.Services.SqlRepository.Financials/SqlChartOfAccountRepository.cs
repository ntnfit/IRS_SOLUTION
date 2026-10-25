using Apzon.Services.Repository.Financials;

namespace Apzon.Services.SqlRepository.Financials;

public class SqlChartOfAccountRepository : ChartOfAccountRepository
{
	public SqlChartOfAccountRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
