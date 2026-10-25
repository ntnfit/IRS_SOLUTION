using Apzon.Services.Repository.Administrator.Setup.Purchasing;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Purchasing;

public class HanaLandedCostsSetupRepository : LandedCostsSetupRepository
{
	public HanaLandedCostsSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
