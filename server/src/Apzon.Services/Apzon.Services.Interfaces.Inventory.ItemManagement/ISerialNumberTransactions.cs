using System.Data;

namespace Apzon.Services.Interfaces.Inventory.ItemManagement;

public interface ISerialNumberTransactions
{
	DataTable GetSerialNumbers(string itemCode, int sysNumber);

	DataTable GetTransactions(string itemCode, int sysNumber, string whsCode);

	DataTable GetDocs(string distNumber, string itemfrom, string itemto, string status);
}
