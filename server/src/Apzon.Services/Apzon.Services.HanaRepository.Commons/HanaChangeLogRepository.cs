using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaChangeLogRepository : ChangeLogRepository
{
	public HanaChangeLogRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
