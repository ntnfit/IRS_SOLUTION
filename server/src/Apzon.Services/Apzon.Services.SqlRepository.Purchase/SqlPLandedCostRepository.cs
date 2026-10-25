using Apzon.Services.Repository.Purchase;

namespace Apzon.Services.SqlRepository.Purchase;

public class SqlPLandedCostRepository : PLandedCostRepository
{
	public SqlPLandedCostRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
