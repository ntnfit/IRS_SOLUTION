using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaSalesForeCastRepository : SalesForeCastRepository
{
	public HanaSalesForeCastRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
