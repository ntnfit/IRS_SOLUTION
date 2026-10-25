using Apzon.Commons.Enums;
using Apzon.Entities.Models.MasterData;

namespace Apzon.DataServices.Interfaces;

public interface IWarehouses : IRepository<WarehouseImpl>
{
	string Save(WarehouseImpl item, string sessionId, Enums.TransactionType tranType);

	string Remove(string itemCode, string sessionId);
}
