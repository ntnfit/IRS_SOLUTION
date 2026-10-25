using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaUnitOfMeasureRepository : UnitOfMeasureRepository
{
	public HanaUnitOfMeasureRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
