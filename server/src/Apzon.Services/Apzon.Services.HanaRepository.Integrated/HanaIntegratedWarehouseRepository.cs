using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.HanaRepository.Integrated;

public class HanaIntegratedWarehouseRepository : IntegratedWarehouseRepository
{
	public HanaIntegratedWarehouseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
