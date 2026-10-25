using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlJetstarPreOrderRepository : JetstarPreOrderRepository
{
	public SqlJetstarPreOrderRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
