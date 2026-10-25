using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.SqlRepository.Inventory.InventoryTransaction;

public class SqlGoodsIssueItRespository : GoodsIssueItRespository
{
	public SqlGoodsIssueItRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
