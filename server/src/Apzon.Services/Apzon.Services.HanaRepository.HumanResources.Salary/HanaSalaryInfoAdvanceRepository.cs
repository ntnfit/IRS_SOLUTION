using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaSalaryInfoAdvanceRepository : SalaryInfoAdvanceRepository
{
	public HanaSalaryInfoAdvanceRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
