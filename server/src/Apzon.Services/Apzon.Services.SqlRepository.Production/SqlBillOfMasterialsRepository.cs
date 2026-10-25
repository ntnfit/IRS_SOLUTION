using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlBillOfMasterialsRepository : BillOfMasterialsRepository
{
	public SqlBillOfMasterialsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
