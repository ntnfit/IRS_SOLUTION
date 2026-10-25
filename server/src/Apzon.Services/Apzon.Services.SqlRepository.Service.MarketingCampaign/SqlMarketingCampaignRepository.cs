using Apzon.Services.Repository.Service.MarketingCampaign;

namespace Apzon.Services.SqlRepository.Service.MarketingCampaign;

public class SqlMarketingCampaignRepository : MarketingCampaignRepository
{
	public SqlMarketingCampaignRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
