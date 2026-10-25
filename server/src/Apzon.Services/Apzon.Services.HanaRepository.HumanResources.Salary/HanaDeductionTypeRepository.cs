using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaDeductionTypeRepository : DeductionTypeRepository
{
	public HanaDeductionTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
