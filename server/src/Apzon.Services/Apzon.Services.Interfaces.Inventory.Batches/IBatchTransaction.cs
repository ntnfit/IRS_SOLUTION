using System.Data;

namespace Apzon.Services.Interfaces.Inventory.Batches;

public interface IBatchTransaction
{
	DataTable GetDocs(string distNumber, string itemfrom, string itemto, string status);

	DataTable GetDoc(string itemCode, string sysNumber, string locCode);
}
