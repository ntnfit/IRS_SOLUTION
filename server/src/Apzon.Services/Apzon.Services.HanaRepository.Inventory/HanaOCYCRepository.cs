using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaOCYCRepository : OCYCRepository
{
	public HanaOCYCRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
