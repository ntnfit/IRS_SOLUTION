using Apzon.Services.Repository.Sales;

namespace Apzon.Services.HanaRepository.Sales;

public class HanaDeliveryControlRepository : DeliveryControlRepository
{
	public HanaDeliveryControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
