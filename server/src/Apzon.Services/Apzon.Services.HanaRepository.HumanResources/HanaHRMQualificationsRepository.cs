using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaHRMQualificationsRepository : HRMQualificationsRepository
{
	public HanaHRMQualificationsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
