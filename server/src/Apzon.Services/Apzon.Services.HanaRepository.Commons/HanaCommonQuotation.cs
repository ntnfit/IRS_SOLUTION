using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaCommonQuotation : CommonQuotation
{
	public HanaCommonQuotation(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
