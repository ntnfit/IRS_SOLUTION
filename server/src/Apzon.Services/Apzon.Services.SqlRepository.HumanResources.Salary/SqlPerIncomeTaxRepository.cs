using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.SqlRepository.HumanResources.Salary;

public class SqlPerIncomeTaxRepository : PerIncomeTaxRepository
{
	public SqlPerIncomeTaxRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
