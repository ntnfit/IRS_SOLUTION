using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaScheduleRepository : ScheduleRepository
{
	public HanaScheduleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNewIdScheduleQuery()
	{
		return "select ifnull((Select max(\"PSCode\") from APZ_OPRS), 0) from DUMMY;";
	}
}
