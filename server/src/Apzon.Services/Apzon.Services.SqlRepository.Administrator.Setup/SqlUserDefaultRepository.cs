using System.Data;
using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlUserDefaultRepository(IDatabaseService databaseService) : UserDefaultRepository(databaseService)
{
	private DataTable dt = new DataTable();
}
