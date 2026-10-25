using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlVehicleRepository : VehicleRepository
{
	private const string Doctemp = "#APZ_VHCE";

	public SqlVehicleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
