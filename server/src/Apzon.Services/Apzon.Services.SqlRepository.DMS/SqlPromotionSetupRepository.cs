using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlPromotionSetupRepository : PromotionSetupRepository
{
	public SqlPromotionSetupRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
