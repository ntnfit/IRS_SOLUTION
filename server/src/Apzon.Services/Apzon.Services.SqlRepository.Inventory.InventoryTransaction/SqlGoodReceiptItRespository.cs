using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlGoodReceiptItRespository : GoodReceiptItRespository
{
	public SqlGoodReceiptItRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
