using Apzon.Services.Repository.Sales;

namespace Apzon.Services.SqlRepository.Sales;

public class SqlDeliveryControlRepository : DeliveryControlRepository
{
	public SqlDeliveryControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
