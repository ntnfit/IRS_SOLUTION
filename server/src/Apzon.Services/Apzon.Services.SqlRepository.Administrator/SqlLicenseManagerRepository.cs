using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

public class SqlLicenseManagerRepository : LicenseManagerRepository
{
	public SqlLicenseManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
