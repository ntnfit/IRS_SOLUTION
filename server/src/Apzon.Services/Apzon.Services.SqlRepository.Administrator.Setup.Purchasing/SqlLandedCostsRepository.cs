using Apzon.Services.Repository.Administrator.Setup.Purchasing;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Purchasing;

public class SqlLandedCostsRepository : LandedCostsRepository
{
	public SqlLandedCostsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
