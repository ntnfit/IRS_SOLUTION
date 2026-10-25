using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryCalculationRepository : SalaryCalculationRepository
{
	public SqlSalaryCalculationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
