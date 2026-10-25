using Apzon.Services.Repository.Service.MarketingCampaign;

namespace Apzon.Services.HanaRepository.Service.MarketingCampaign;

public class HanaMarketingCampaignRepository : MarketingCampaignRepository
{
	public HanaMarketingCampaignRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxIdQuery()
	{
		return "Select Max(\"WrntyID\") from APZ_OSMC";
	}
}
