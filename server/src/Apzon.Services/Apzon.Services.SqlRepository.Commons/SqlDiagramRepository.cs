using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlDiagramRepository : DiagramRepository
{
	public SqlDiagramRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
