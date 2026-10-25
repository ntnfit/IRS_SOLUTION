using Apzon.Services.Repository.Sales;

namespace Apzon.Services.HanaRepository.Sales;

public class HanaDeliveryConfirmRepository : DeliveryConfirmRepository
{
	public HanaDeliveryConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
