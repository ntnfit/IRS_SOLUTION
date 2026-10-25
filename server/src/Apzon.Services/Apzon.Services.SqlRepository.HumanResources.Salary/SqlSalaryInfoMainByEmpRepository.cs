using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryInfoMainByEmpRepository : SalaryInfoMainByEmpRepository
{
	public SqlSalaryInfoMainByEmpRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
