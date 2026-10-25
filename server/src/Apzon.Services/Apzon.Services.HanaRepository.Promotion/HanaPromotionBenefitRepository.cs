using Apzon.Services.Repository.Promotion;

namespace Apzon.Services.HanaRepository.Promotion;

internal class HanaPromotionBenefitRepository : PromotionBenefitRepository
{
	public HanaPromotionBenefitRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
