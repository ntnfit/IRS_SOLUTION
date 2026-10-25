using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlUnitOfMeasureRepository(IDatabaseService databaseService) : UnitOfMeasureRepository(databaseService)
{
	public new string TempTable = "#OUOM";
}
