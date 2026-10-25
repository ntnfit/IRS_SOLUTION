using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaTerritoryRepository : TerritoryRepository
{
	public HanaTerritoryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
