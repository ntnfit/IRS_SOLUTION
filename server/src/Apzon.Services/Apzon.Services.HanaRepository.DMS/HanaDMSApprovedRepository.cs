using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaDMSApprovedRepository : DMSApprovedRepository
{
	public HanaDMSApprovedRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
