using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlXtraReportManagerRepository : XtraReportManagerRepository
{
	public SqlXtraReportManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
