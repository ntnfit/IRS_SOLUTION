using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaAllowanceTypeRepository : AllowanceTypeRepository
{
	public HanaAllowanceTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
