using Apzon.Services.Repository.Financials;

namespace Apzon.Services.SqlRepository.Financials;

public class SqlCostCentersRepository : CostCentersRepository
{
	public SqlCostCentersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
