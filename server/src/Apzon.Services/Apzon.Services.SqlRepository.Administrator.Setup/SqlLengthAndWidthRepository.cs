using System.Data;
using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlLengthAndWidthRepository(IDatabaseService databaseService) : LengthAndWidthRepository(databaseService)
{
	private DataTable dt = new DataTable();
}
