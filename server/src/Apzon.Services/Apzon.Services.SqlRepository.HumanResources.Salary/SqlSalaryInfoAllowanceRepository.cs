using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryInfoAllowanceRepository : SalaryInfoAllowanceRepository
{
	public SqlSalaryInfoAllowanceRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
