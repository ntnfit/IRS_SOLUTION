using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlUDFRepository : UDFRepository
{
	public SqlUDFRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
