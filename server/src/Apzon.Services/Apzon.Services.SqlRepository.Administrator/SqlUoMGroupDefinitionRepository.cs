using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlUoMGroupDefinitionRepository(IDatabaseService databaseService) : UoMGroupDefinitionRepository(databaseService)
{
	public new string TempTable = "#UGP1";
}
