using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.SqlRepository.PackingManagement;

internal class SqlPrintBillWrapRepository : PrintBillWrapRepository
{
	public SqlPrintBillWrapRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
