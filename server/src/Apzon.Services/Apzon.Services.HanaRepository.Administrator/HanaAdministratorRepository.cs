using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaAdministratorRepository : AdministratorRepository
{
	public HanaAdministratorRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
