using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaSalaryAllowanceByEmpRepository : SalaryAllowanceByEmpRepository
{
	public HanaSalaryAllowanceByEmpRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
