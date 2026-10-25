using Apzon.Services.Repository.Administrator.Business_Partner;

namespace Apzon.Services.HanaRepository.Administrator.Business_Partner;

public class HanaCountriesRepository : CountriesRepository
{
	public HanaCountriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
