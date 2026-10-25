using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlAuthRepository : AuthRepository
{
	public SqlAuthRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
