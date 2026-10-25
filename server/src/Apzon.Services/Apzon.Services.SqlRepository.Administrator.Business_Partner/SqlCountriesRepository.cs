using Apzon.Services.Repository.Administrator.Business_Partner;

namespace Apzon.Services.SqlRepository.Administrator.Business_Partner;

public class SqlCountriesRepository : CountriesRepository
{
	public SqlCountriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
