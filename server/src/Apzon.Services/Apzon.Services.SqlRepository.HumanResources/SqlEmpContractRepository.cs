using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlEmpContractRepository : EmpContractRepository
{
	public SqlEmpContractRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
