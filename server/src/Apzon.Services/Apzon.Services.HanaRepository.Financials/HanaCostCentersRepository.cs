using Apzon.Services.Repository.Financials;

namespace Apzon.Services.HanaRepository.Financials;

public class HanaCostCentersRepository : CostCentersRepository
{
	public HanaCostCentersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
