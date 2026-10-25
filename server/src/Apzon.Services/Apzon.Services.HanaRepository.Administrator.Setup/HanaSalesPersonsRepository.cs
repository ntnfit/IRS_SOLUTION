using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaSalesPersonsRepository : SalesPersonsRepository
{
	public HanaSalesPersonsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
