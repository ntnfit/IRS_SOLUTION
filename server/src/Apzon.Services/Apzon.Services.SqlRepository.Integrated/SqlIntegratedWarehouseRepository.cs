using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.SqlRepository.Integrated;

public class SqlIntegratedWarehouseRepository : IntegratedWarehouseRepository
{
	public SqlIntegratedWarehouseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
