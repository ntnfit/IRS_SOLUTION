using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlManufacturersRepository : ManufacturersRepository
{
	public SqlManufacturersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
