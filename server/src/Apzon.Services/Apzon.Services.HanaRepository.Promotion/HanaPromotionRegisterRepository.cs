using Apzon.Services.Repository.Promotion;

namespace Apzon.Services.HanaRepository.Promotion;

internal class HanaPromotionRegisterRepository : PromotionRegisterRepository
{
	public HanaPromotionRegisterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
