using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaUsersIRSRepository : UsersIRSRepository
{
	public HanaUsersIRSRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
