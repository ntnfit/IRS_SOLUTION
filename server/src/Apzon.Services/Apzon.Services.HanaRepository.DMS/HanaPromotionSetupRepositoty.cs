using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaPromotionSetupRepositoty : PromotionSetupRepository
{
	public HanaPromotionSetupRepositoty(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
