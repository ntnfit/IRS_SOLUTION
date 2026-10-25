using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlCommonQuotation : CommonQuotation
{
	public SqlCommonQuotation(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
