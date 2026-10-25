using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaServiceLevelAgreementRepository : ServiceLevelAgreementRepository
{
	public HanaServiceLevelAgreementRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
