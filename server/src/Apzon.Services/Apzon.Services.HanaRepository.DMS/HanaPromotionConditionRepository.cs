using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaPromotionConditionRepository : PromotionConditionRepository
{
	public HanaPromotionConditionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
