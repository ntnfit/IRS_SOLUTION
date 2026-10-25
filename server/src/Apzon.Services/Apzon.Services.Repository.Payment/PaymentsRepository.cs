using System.Data;
using Apzon.Services.Interfaces.Payment;

namespace Apzon.Services.Repository.Payment;

internal class PaymentsRepository : BaseService<DataSet>, IPayments
{
	public PaymentsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetById(int docEntry)
	{
		return new DataTable();
	}
}
