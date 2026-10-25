using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaSalaryFormulaRepository : SalaryFormulaRepository
{
	public HanaSalaryFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
