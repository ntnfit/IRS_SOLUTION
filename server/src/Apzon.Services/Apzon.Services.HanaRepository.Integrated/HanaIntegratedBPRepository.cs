using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.HanaRepository.Integrated;

public class HanaIntegratedBPRepository : IntegratedBPRepository
{
	public HanaIntegratedBPRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
