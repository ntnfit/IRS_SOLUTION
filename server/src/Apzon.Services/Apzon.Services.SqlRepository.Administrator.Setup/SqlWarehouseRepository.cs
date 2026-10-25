using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlWarehouseRepository(IDatabaseService databaseService) : WarehouseRepository(databaseService)
{
	private string TempWHSTableName = "#APZ_WAREHOUSE_DOC";
}
