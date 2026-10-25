using Apzon.Services.Repository.Promotion;

namespace Apzon.Services.HanaRepository.Promotion;

internal class HanaPromotionProgramRepository : PromotionProgramRepository
{
	public HanaPromotionProgramRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
