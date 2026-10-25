using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.HanaRepository.Purchase;

public class HanaPLandedCostRepository : PLandedCostRepository
{
	public HanaPLandedCostRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
