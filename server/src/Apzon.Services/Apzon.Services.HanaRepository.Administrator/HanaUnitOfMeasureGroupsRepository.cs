using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaUnitOfMeasureGroupsRepository : UnitOfMeasureGroupsRepository
{
	public HanaUnitOfMeasureGroupsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
