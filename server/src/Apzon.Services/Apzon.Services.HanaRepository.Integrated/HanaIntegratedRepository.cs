using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.HanaRepository.Integrated;

public class HanaIntegratedRepository : IntegratedRepository
{
	public HanaIntegratedRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
