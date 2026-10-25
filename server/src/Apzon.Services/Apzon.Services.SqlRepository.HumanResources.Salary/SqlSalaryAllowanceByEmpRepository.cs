using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryAllowanceByEmpRepository : SalaryAllowanceByEmpRepository
{
	public SqlSalaryAllowanceByEmpRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
