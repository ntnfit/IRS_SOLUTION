using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

public class SqlAdministratorRepository : AdministratorRepository
{
	public SqlAdministratorRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
