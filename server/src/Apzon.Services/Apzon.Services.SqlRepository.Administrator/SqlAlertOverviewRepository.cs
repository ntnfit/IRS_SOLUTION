using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlAlertOverviewRepository : AlertOverviewRepository
{
	public SqlAlertOverviewRepository(IDatabaseService databaseService, IUnitOfWork unitOfWork)
		: base(databaseService, unitOfWork)
	{
	}
}
