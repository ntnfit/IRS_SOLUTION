using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaMaterialShortageReportRepository : MaterialShortageReportRepository
{
	public HanaMaterialShortageReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
