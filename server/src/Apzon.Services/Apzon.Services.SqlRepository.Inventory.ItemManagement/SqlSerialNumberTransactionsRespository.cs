using Apzon.Services.Repository.Inventory.ItemManagement;

namespace Apzon.Services.SqlRepository.Inventory.ItemManagement;

public class SqlSerialNumberTransactionsRespository : SerialNumberTransactionsRespository
{
	public SqlSerialNumberTransactionsRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
