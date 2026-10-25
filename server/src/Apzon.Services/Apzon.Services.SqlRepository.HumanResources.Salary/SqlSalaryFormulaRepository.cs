using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryFormulaRepository : SalaryFormulaRepository
{
	public SqlSalaryFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
