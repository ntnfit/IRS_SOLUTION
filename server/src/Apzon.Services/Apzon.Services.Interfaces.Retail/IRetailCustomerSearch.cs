using System.Data;

namespace Apzon.Services.Interfaces.Retail;

public interface IRetailCustomerSearch
{
	DataTable LoadDoc(string cardType, string userName, string branch);
}
