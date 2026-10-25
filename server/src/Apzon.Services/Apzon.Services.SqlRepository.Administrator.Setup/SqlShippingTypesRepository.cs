using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlShippingTypesRepository : ShippingTypesRepository
{
	public SqlShippingTypesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
