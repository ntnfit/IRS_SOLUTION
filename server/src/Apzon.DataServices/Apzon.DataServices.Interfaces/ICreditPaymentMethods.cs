using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface ICreditPaymentMethods : IRepository<DataTable>
{
	string Save(DataTable data, string sessionId);

	string DeleteLine(int crTypeCode, string sessionId);
}
