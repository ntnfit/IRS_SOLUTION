using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlDepartmentRepository : DepartmentRepository
{
	public SqlDepartmentRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
