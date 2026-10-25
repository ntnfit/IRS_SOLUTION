using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlServiceLevelAgreementRepository : ServiceLevelAgreementRepository
{
	public SqlServiceLevelAgreementRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
