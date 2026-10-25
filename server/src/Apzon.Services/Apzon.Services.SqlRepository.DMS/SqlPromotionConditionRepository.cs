using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlPromotionConditionRepository : PromotionConditionRepository
{
	public SqlPromotionConditionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
