using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlServiceContractTemplateRepository : ServiceContractTemplateRepository
{
	public SqlServiceContractTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
