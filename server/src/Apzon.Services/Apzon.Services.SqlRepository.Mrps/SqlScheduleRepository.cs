using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlScheduleRepository : ScheduleRepository
{
	public SqlScheduleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
