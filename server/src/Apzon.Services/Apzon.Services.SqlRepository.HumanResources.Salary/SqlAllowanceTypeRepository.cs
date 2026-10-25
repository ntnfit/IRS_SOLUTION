using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlAllowanceTypeRepository : AllowanceTypeRepository
{
	public SqlAllowanceTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
