using System.Data;

namespace Apzon.Services.Interfaces.Retail;

public interface IRetailItemSearch
{
	DataTable LoadDoc(string itemCode, string itemType, string userName, string branch, string pr);
}
