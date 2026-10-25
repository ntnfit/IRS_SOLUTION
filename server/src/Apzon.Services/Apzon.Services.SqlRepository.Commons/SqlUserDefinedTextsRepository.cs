using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlUserDefinedTextsRepository : UserDefinedTextsRepository
{
	public SqlUserDefinedTextsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
