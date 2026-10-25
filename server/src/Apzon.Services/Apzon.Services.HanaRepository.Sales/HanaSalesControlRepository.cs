using Apzon.Services.Repository.Sales;

namespace Apzon.Services.HanaRepository.Sales;

public class HanaSalesControlRepository : SalesControlRepository
{
	public HanaSalesControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
