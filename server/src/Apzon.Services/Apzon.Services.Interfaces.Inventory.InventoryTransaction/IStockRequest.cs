using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Inventory.InventoryTransaction;

public interface IStockRequest : IService<DocumentsImpl>
{
	DataSet GetAll(DataTable dt);

	DataSet GetDetail(int docentry);

	DataTable CheckRequest(int docForBatch, string username, int branch);

	string UpdatePrintCount(string docentry, string table, int count, string userName = "");
}
