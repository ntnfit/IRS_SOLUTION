using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlDeductionTypeRepository : DeductionTypeRepository
{
	public SqlDeductionTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
