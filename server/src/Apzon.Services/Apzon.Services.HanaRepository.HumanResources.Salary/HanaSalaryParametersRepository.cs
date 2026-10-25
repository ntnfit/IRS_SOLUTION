using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaSalaryParametersRepository : SalaryParametersRepository
{
	public HanaSalaryParametersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
