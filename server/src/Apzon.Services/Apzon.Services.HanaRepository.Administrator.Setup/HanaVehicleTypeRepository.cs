using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaVehicleTypeRepository : VehicleTypeRepository
{
	public HanaVehicleTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
