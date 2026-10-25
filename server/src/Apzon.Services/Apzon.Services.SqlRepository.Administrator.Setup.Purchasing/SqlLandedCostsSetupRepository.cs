using Apzon.Services.Repository.Administrator.Setup.Purchasing;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Purchasing;

public class SqlLandedCostsSetupRepository : LandedCostsSetupRepository
{
	public SqlLandedCostsSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
