using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaSalaryCalculationRepository : SalaryCalculationRepository
{
	public HanaSalaryCalculationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
