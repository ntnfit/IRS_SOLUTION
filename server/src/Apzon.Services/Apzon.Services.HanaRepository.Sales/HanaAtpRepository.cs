using Apzon.Services.Repository.Sales;

namespace Apzon.Services.HanaRepository.Sales;

public class HanaAtpRepository : AtpRepository
{
	public HanaAtpRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
