using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlEmpRelativesRepository : EmpRelativesRepository
{
	public SqlEmpRelativesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
