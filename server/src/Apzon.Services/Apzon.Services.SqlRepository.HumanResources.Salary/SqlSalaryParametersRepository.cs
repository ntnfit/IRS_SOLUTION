using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryParametersRepository : SalaryParametersRepository
{
	public SqlSalaryParametersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
