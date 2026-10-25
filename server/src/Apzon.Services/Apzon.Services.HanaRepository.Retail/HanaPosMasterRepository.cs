using Apzon.Services.Repository.Retail;

namespace Apzon.Services.HanaRepository.Retail;

public class HanaPosMasterRepository : PosMasterRepository
{
	public HanaPosMasterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
