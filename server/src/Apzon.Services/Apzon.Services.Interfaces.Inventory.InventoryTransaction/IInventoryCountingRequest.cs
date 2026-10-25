using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IInventoryCountingRequest
{
	DataTable LoadData(int id, string whs, string startFrom, string startTo);

	HttpResult Saves(DataTable dt);

	string DeleteDocs(string ids);

	DataTable FindNextLastFrist(int docId, int type);
}
