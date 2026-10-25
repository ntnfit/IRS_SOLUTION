using Apzon.Services.Repository.HumanResources.Salary;

namespace Apzon.Services.HanaRepository.HumanResources.Salary;

public class HanaPerIncomeTaxRepository : PerIncomeTaxRepository
{
	public HanaPerIncomeTaxRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
