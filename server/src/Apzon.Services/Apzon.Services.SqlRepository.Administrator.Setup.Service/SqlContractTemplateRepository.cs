using Apzon.Services.Repository.Administrator.Setup.Service;

namespace Apzon.Services.SqlRepository.Administrator.Setup.Service;

public class SqlContractTemplateRepository : ContractTemplateRepository
{
	public SqlContractTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
