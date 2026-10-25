using Apzon.Services.Repository.Administrator.Setup.Service;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Service;

public class HanaContractTemplateRepository : ContractTemplateRepository
{
	public HanaContractTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
