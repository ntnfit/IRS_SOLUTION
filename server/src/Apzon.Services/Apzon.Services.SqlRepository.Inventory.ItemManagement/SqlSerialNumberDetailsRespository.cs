using Apzon.Services.Repository.Inventory.ItemManagement;

namespace Apzon.Services.SqlRepository.Inventory.ItemManagement;

public class SqlSerialNumberDetailsRespository : SerialNumberDetailsRespository
{
	public SqlSerialNumberDetailsRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
