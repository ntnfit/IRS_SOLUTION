using Apzon.Services.Repository.Financials;

namespace Apzon.Services.SqlRepository.Financials;

public class SqlDocumentConfirmRepository : DocumentConfirmRepository
{
	public SqlDocumentConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
