using Apzon.Services.Repository.Financials;

namespace Apzon.Services.SqlRepository.Financials;

public class SqlCurrenciesRepository : CurrenciesRepository
{
	public SqlCurrenciesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
