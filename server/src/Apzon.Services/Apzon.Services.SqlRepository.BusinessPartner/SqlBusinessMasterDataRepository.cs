using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlBusinessMasterDataRepository : BusinessMasterDataRepository
{
	public SqlBusinessMasterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
