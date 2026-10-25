using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaBomJobRepository : BoMJobRepository
{
	public HanaBomJobRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
