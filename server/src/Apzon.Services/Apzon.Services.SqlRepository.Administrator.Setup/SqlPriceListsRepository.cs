using System.Data;
using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlPriceListsRepository(IDatabaseService databaseService) : PriceListsRepository(databaseService)
{
	private DataTable dt = new DataTable();
}
