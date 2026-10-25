using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlSalaryInfoRepository : SalaryInfoRepository
{
	public SqlSalaryInfoRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
