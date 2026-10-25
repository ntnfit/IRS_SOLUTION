using Apzon.Services.Repository.Sales;

namespace Apzon.Services.SqlRepository.Sales;

public class SqlDeliveryConfirmRepository : DeliveryConfirmRepository
{
	public SqlDeliveryConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
