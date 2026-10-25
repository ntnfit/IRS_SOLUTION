using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryInfoAdvanceRepository : SalaryInfoAdvanceRepository
{
	public SqlSalaryInfoAdvanceRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
