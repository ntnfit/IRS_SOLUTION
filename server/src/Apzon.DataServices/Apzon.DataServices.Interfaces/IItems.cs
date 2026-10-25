using System.Collections.Generic;
using Apzon.Entities.Models.MasterData;

namespace Apzon.DataServices.Interfaces;

public interface IItems : IRepository<ItemsImpl>
{
	string Save(ItemsImpl item, string sessionId, bool ignoreEmptyValue = false);

	string UpdateUoMPrices(List<Items_UoMPricesImpl> lst, string sessionId);

	string Remove(string itemCode, string sessionId);
}
