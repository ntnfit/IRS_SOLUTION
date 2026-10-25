using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.SqlRepository.HumanResources;

public class SqlHRMQualificationsRepository : HRMQualificationsRepository
{
	public SqlHRMQualificationsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
