using Apzon.Services.Repository.Integrated;

namespace Apzon.Services.HanaRepository.Integrated;

public class HanaIntegratedItemRepository : IntegratedItemRepository
{
	public HanaIntegratedItemRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
