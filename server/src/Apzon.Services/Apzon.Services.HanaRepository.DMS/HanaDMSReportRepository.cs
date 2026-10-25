using Apzon.Services.Repository.DMS;

namespace Apzon.Services.HanaRepository.DMS;

internal class HanaDMSReportRepository : DMSReportRepository
{
	public HanaDMSReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
