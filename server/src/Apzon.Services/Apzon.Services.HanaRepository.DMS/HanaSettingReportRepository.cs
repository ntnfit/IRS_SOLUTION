using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

public class HanaSettingReportRepository : SettingReportRepository
{
	public HanaSettingReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNoQuery()
	{
		return "select ifnull(max(\"DocEntry\"), 0) + 1 from APZ_D_OSRT;";
	}
}
