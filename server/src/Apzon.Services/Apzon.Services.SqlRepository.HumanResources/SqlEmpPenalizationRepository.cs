using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlEmpPenalizationRepository : EmpPenalizationRepository
{
	public SqlEmpPenalizationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
