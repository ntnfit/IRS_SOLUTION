using Apzon.Services.Repository.Promotion;

namespace Apzon.Services.HanaRepository.Promotion;

internal class HanPromotionRepository : PromotionRepository
{
	public HanPromotionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
