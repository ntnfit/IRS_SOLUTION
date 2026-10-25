using System.Data;
using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlStateRepository(IDatabaseService databaseService) : StateRepository(databaseService)
{
	private DataTable dt = new DataTable();
}
