using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.SqlRepository.BusinessPartner;

public class SqlContactPersonRepository : ContactPersonRepository
{
	public SqlContactPersonRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
