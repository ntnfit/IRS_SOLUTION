using Apzon.Services.Repository.Financials;

namespace Apzon.Services.HanaRepository.Financials;

public class HanaCurrenciesRepository : CurrenciesRepository
{
	public HanaCurrenciesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
