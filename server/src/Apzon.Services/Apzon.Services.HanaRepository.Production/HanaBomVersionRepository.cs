using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaBomVersionRepository : BomVersionRepository
{
	public HanaBomVersionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
