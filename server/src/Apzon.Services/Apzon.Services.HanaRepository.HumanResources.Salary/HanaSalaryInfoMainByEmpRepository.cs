using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaSalaryInfoMainByEmpRepository : SalaryInfoMainByEmpRepository
{
	public HanaSalaryInfoMainByEmpRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
