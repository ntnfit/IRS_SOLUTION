using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.SqlRepository.Administrator.SystemInitialization;

public class SqlCompanyDetailsRepository : CompanyDetailsRepository
{
	public SqlCompanyDetailsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
