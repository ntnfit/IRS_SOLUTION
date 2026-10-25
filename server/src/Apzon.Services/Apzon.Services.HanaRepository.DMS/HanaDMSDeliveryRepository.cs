using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaDMSDeliveryRepository : DMSDeliveryRepository
{
	public HanaDMSDeliveryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
