using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlVehicleTypeRepository : VehicleTypeRepository
{
	private const string Doctemp = "#APZ_VHCT";

	public SqlVehicleTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
