using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface ICreditCard : IRepository<DataTable>
{
	string Save(DataTable data, string sessionId);
}
