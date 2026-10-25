using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlEmpMasterDataRepository : EmpMasterDataRepository
{
	public SqlEmpMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
