using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaFreightRepository : FreightRepository
{
	public HanaFreightRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
