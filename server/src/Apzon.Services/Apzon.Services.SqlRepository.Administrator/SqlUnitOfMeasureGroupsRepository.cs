using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlUnitOfMeasureGroupsRepository(IDatabaseService databaseService) : UnitOfMeasureGroupsRepository(databaseService)
{
	public new string TempTable = "#OUGP";
}
